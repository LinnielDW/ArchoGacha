using System;
using System.Collections.Generic;
using System.Linq;
using ArchoGacha.UI;
using ArchoGacha.Utils;
using RimWorld;
using Verse;
using static ArchoGacha.ArchoGachaMod;

namespace ArchoGacha.MapComponents;

public class MapComponentGachaTracker : GameComponent
{
    public List<PrizeBanner> activeBanners = new();
    public float pitySilverReserve;
    private int bannersEndTick;
    private bool fiftyFifty;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref activeBanners, "activeBanners", true,
            LookMode.Deep);
        Scribe_Values.Look(ref pitySilverReserve, "pitySilverReserve");
        Scribe_Values.Look(ref bannersEndTick, "bannersEndTick");
        Scribe_Values.Look(ref fiftyFifty, "fiftyFifty");
    }

    public override void GameComponentTick()
    {
        base.GameComponentTick();
        if (GenTicks.IsTickInterval(1000))
        {
            if (activeBanners.NullOrEmpty() ||
                bannersEndTick <= Find.TickManager.TicksGame)
            {
                GenerateActiveBanners();
            }
            //TODO: implement cleanup and handling pity (if needed), etc
        }
    }

    public void GenerateActiveBanners()
    {
        activeBanners.Clear();
        Dialog_BannerMenu.selectedBanner = null;
        foreach (var b in DefDatabase<PrizeBannerDef>.AllDefsListForReading)
        {
            for (var i = 0; i < b.spawnCountRange.RandomInRange; i++)
            {
                activeBanners.Add(GenerateBannerFromDef(b));
            }
        }

        bannersEndTick = (int)(Find.TickManager.TicksGame +
                               60000 * settings.bannerDurationDays);

        //TODO: add translation strings
        Find.LetterStack.ReceiveLetter(
            "ArchoGacha_BannersRefreshedLabel".Translate(),
            "ArchoGacha_BannersRefreshed".Translate(),
            LetterDefOf.PositiveEvent);

        //TODO: add debug info setting
        /*foreach (var banner in activeBanners)
        {
            Log.Message(banner.ToString());
        }*/
    }

    public PrizeBanner GenerateRandomBanner()
    {
        return GenerateBannerFromDef(DefDatabase<PrizeBannerDef>
            .AllDefsListForReading.RandomElement());
    }

    private PrizeBanner GenerateBannerFromDef(PrizeBannerDef def)
    {
        var prizeBanner =
            (PrizeBanner)Activator.CreateInstance(def.prizeBannerClass);

        prizeBanner.def = def;
        prizeBanner.jackpot = prizeBanner.GenerateJackpot();
        prizeBanner.consolationPrizes = GenerateConsolations(prizeBanner, prizeBanner.jackpot.MarketValue * settings.consolationChance)
            .ToList();
        prizeBanner.pullPrice = (int)(prizeBanner.jackpot.MarketValue *
                                      settings.jackpotChance *
                                      settings.pullPriceFactor);

        return prizeBanner;
    }

    private static IEnumerable<Thing> GenerateConsolations(
        PrizeBanner prizeBanner,
        float valueMaxOverride = 0f)
    {
        for (int i = 0;
             i < prizeBanner.def.consolationCountRange.RandomInRange;
             i++)
        {
            var prize = prizeBanner.GenerateConsolationThing(valueMaxOverride);
            if (prize != null) yield return prize;
        }
    }

    public void PullOnBanner(PrizeBanner prizeBanner)
    {
        if (CanPullOnBanner(prizeBanner))
        {
            DoPull(prizeBanner);
        }
        //TODO: push notification/letter on banner result
    }

    public void PullTenOnBanner(PrizeBanner prizeBanner)
    {
        if (CanPullTenOnBanner(prizeBanner))
        {
            for (int i = 0; i < 10; i++)
            {
                DoPull(prizeBanner);
            }
            //TODO: push notification/letter on banner result
        }
    }

    private void DoPull(PrizeBanner prizeBanner)
    {
        TradeUtility.LaunchSilver(Find.CurrentMap, (int)prizeBanner.pullPrice);
        pitySilverReserve += prizeBanner.pullPrice;
        // pityCount++;
        var prize = DecidePrize(prizeBanner);
        DeliverPrize(prize);
    }

    public bool CanPullOnBanner(PrizeBanner prizeBanner)
    {
        return prizeBanner.CanPull(Find.CurrentMap) ||
               Prefs.DevMode && settings.debugAlwaysPullable;
    }    
    public bool CanPullTenOnBanner(PrizeBanner prizeBanner)
    {
        return prizeBanner.CanPullTen(Find.CurrentMap) ||
               Prefs.DevMode && settings.debugAlwaysPullable;
    }

    public void DeliverPrize(Thing prize)
    {
        TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(Find.CurrentMap), Find.CurrentMap,
            prize);
    }

    public Thing DecidePrize(PrizeBanner prizeBanner)
    {
        var randOutcome = Rand.Value;
        if (Prefs.DevMode)
        {
            Log.Message($"Rand outcome: {randOutcome}");
        }

        switch (randOutcome)
        {
            case var _ when randOutcome < settings.jackpotChance || pitySilverReserve >= prizeBanner.PityThreshold:
            {
                // pityCount = 0;
                var jackpot = SelectJackpot(prizeBanner);
                pitySilverReserve = Math.Max(0, pitySilverReserve - (int)(jackpot.MarketValue * 2f));
                return jackpot;
            }
            case var _ when randOutcome < settings.consolationChance:
            {
                // pityCount = 0;
                return SelectConsolation(prizeBanner);
            }
            default:
            {
                //TODO: move to banner
                return ThingMaker.MakeThing(ThingDefOf.WoodLog);
            }
        }
    }

    //public int pityCount = 0;

    
    public Thing SelectJackpot(PrizeBanner prizeBanner)
    {
        if ((Rand.Bool || fiftyFifty) && prizeBanner.jackpot != null)
        {
            if (Prefs.DevMode)
            {
                Log.Message("Won 50/50, deploying new item:");
            }

            var prize = prizeBanner.jackpot;
            prizeBanner.jackpot = null;
            return prize;
        }
        
        if (Prefs.DevMode)
        {
            Log.Message("Lost 50/50. Generating new jackpot item:");
        }

        if(prizeBanner.jackpot != null)
        {
            fiftyFifty = true;
        }

        return prizeBanner.GeneratePrize(PrizeCategory.Jackpot, prizeBanner.PityThreshold);
    }

    public Thing SelectConsolation(PrizeBanner prizeBanner)
    {
        if (Rand.Bool && !prizeBanner.consolationPrizes.NullOrEmpty())
        {
            if (Prefs.DevMode)
            {
                Log.Message("Won 50/50, deploying new item:");
            }

            var prize = prizeBanner.consolationPrizes.RandomElement();
            prizeBanner.consolationPrizes.Remove(prize);
            return prize;
        }
        
        if (Prefs.DevMode)
        {
            Log.Message("Won 50/50, deploying new item:");
        }
        return prizeBanner.GeneratePrize(PrizeCategory.Consolation,
            prizeBanner.PityThreshold * settings.consolationChance);
    }

    public MapComponentGachaTracker(): base()
    {
    }
    public MapComponentGachaTracker(Game game)
    {
    }
}