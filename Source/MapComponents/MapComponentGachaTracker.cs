using System;
using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Utils;
using RimWorld;
using Verse;
using static ArchoGacha.ArchoGachaMod;

namespace ArchoGacha.MapComponents;

public class MapComponentGachaTracker : MapComponent
{
    public List<PrizeBanner> activeBanners = new();
    public int pitySilverReserve;
    public int bannersEndTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref activeBanners, "activeBanners", true,
            LookMode.Deep);
        Scribe_Values.Look(ref pitySilverReserve, "pitySilverReserve");
        Scribe_Values.Look(ref bannersEndTick, "bannersEndTick");
    }

    public override void MapComponentTick()
    {
        base.MapComponentTick();
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

    public PrizeBanner GenerateBannerFromDef(PrizeBannerDef def)
    {
        var prizeBanner =
            (PrizeBanner)Activator.CreateInstance(def.prizeBannerClass);

        prizeBanner.def = def;
        prizeBanner.jackpot = prizeBanner.GenerateJackpot();
        prizeBanner.consolationPrizes = GenerateConsolations(prizeBanner,
                //TODO: turn multiplier into a setting
                prizeBanner.jackpot.MarketValue * settings.consolationChance)
            .ToList();
        prizeBanner.pullPrice = (int)(prizeBanner.jackpot.MarketValue *
                                      settings.jackpotChance *
                                      settings.pullPriceFactor);

        return prizeBanner;
    }

    public static IEnumerable<Thing> GenerateConsolations(
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
        if (prizeBanner.CanPullOnBanner(map) ||
            Prefs.DevMode && settings.debugAlwaysPullable)
        {
            TradeUtility.LaunchSilver(map, prizeBanner.pullPrice);
            var prize = DecidePrize(prizeBanner);
            DeliverPrize(prize);
            //TODO: increment pity
        }
    }

    public void DeliverPrize(Thing prize)
    {
        TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(map), map,
            prize);
    }

    public Thing DecidePrize(PrizeBanner prizeBanner)
    {
        //todo: check for pity activation
        var randOutcome = Rand.Value;
        if (Prefs.DevMode)
        {
            Log.Message($"Rand outcome: {randOutcome}");
        }
        return randOutcome switch
        {
            _ when randOutcome < settings.jackpotChance => SelectJackpot(
                prizeBanner),
            _ when randOutcome < settings.consolationChance =>
                SelectConsolation(
                    prizeBanner),
            _ => ThingMaker.MakeThing(ThingDefOf.WoodLog)
        };
    }

    public Thing SelectJackpot(PrizeBanner prizeBanner)
    {
        if (Rand.Bool && prizeBanner.jackpot != null)
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
            Log.Message("Lost 50/50. Generating new item:");
        }
        return prizeBanner.GeneratePrize(PrizeCategory.Jackpot,
            //TODO: move this to a var in the prizebanner
            prizeBanner.pullPrice /(
                settings.jackpotChance *
                settings.pullPriceFactor));
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
            //TODO: move this to a var in the prizebanner
            prizeBanner.pullPrice / (
                settings.jackpotChance *
                settings.pullPriceFactor) * settings.consolationChance);
    }

    public MapComponentGachaTracker(Map map) : base(map)
    {
    }
}