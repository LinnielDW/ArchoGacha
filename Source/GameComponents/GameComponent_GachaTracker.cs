using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Defs;
using ArchoGacha.UI;
using ArchoGacha.Utils;

namespace ArchoGacha.GameComponents;

public class GameComponent_GachaTracker : GameComponent
{
    public List<PrizeBanner> activeBanners = new();
    public float pitySilverReserve;
    public int bannersEndTick;
    public bool lostFiftyFifty;

    private List<ThingDef> trash = new();

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        
        trash = DefDatabase<ThingDef>.AllDefs.Where(d =>
            d.category == ThingCategory.Item &&
            d.tradeability.TraderCanSell() &&
            d.equipmentType == EquipmentType.None &&
            d.BaseMarketValue >= 1f &&
            !d.HasComp(typeof(CompHatcher))
        ).ToList();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref activeBanners, "activeBanners", true,
            LookMode.Deep);
        Scribe_Values.Look(ref pitySilverReserve, "pitySilverReserve");
        Scribe_Values.Look(ref bannersEndTick, "bannersEndTick");
        Scribe_Values.Look(ref lostFiftyFifty, "lostFiftyFifty");
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
        }
    }

    public void GenerateActiveBanners()
    {
        activeBanners.Clear();
        Dialog_BannerMenu.selectedBanner = null;

        if (settings.limitBanners && DefDatabase<PrizeBannerDef>.AllDefsListForReading.Sum(b => b.spawnCountRange.max) > settings.bannerLimit)
        {
            while (activeBanners.Count < settings.bannerLimit)
            {
                var banner = DefDatabase<PrizeBannerDef>.AllDefsListForReading.Where(def => def.spawnCountRange.max < activeBanners.Count(activeBanner => activeBanner.def == def)).RandomElement();
                activeBanners.Add(GenerateBannerFromDef(banner));
            }
        }
        else
        {
            foreach (var b in DefDatabase<PrizeBannerDef>.AllDefsListForReading)
            {
                for (var i = 0; i < b.spawnCountRange.RandomInRange; i++)
                {
                    activeBanners.Add(GenerateBannerFromDef(b));
                }
            }
        }

        bannersEndTick = Find.TickManager.TicksGame +
                         (int)(60000f * settings.bannerDurationDays);

        //TODO: add translation strings
        Find.LetterStack.ReceiveLetter(
            "ArchoGacha_BannersRefreshedLabel".Translate(),
            "ArchoGacha_BannersRefreshed".Translate(),
            LetterDefOf.NeutralEvent);

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
        prizeBanner.pullPrice = prizeBanner.jackpot.MarketValue * 
                                prizeBanner.def.valueMultiplier *
                                settings.jackpotChance *
                                settings.pullPriceFactor 
                                * prizeBanner.jackpot.stackCount
                                ;

        AddConsolations(prizeBanner,
            prizeBanner.jackpot.MarketValue * prizeBanner.jackpot.stackCount * settings.consolationChance);
        
        return prizeBanner;
    }

    private static void AddConsolations(PrizeBanner prizeBanner,
        float valueMaxOverride = 0f)
    {        
        for (int i = 0; i < prizeBanner.def.consolationCountRange.RandomInRange; i++)
        {
            var prize = prizeBanner.GenerateConsolationThing(valueMaxOverride);
            if (prize != null) prizeBanner.consolationPrizes.Add(prize);
        }
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
        var mPrize = prize.TryMakeMinified();
        TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(Find.CurrentMap), Find.CurrentMap,
            mPrize);
    }

    public Thing DecidePrize(PrizeBanner prizeBanner)
    {
        var randOutcome = Rand.Value;
        if (Prefs.DevMode)
        {
            // Log.Message($"Rand outcome: {randOutcome}");
        }

        switch (randOutcome)
        {
            case var _ when randOutcome < settings.jackpotChance || pitySilverReserve >= prizeBanner.PityThreshold:
            {
                // pityCount = 0;
                var jackpot = SelectJackpot(prizeBanner);
                pitySilverReserve = Math.Max(0, pitySilverReserve - (int)(jackpot.MarketValue * jackpot.stackCount * prizeBanner.def.valueMultiplier * settings.pullPriceFactor * jackpot.stackCount));
                Find.LetterStack.ReceiveLetter("ArchoGacha_JackpotLetterLabel".Translate(), "ArchoGacha_JackpotLetter".Translate(jackpot.LabelCap),ArchoGachaDefOf.ArchoGacha_Jackpot, jackpot, hyperlinkThingDefs: new List<ThingDef>() {jackpot.def});
                return jackpot;
            }
            case var _ when randOutcome < settings.consolationChance:
            {
                // pityCount = 0;
                var consolation = SelectConsolation(prizeBanner);
                if (consolation != null)
                {
                    Messages.Message("ArchoGacha_Consolation".Translate(consolation.LabelShort),consolation, MessageTypeDefOf.PositiveEvent);
                    return consolation;
                }
                goto default;
            }
            default:
            {
                var trashFiltered = trash
                    .Where(t => t.BaseMarketValue <= prizeBanner.pullPrice/settings.pullPriceFactor)
                    .ToList();
                var trashDef = trashFiltered.NullOrEmpty()
                    ? ThingDefOf.WoodLog
                    : trashFiltered.RandomElement();
                return ThingMaker.MakeThing(trashDef, trashDef.defaultStuff);
            }
        }
    }

    public Thing SelectJackpot(PrizeBanner prizeBanner)
    {
        if ((Rand.Chance(settings.getFeatured) || lostFiftyFifty) && prizeBanner.jackpot != null)
        {
            if (Prefs.DevMode)
            {
                Log.Message("Won 50/50, deploying jackpot item:");
            }

            var prize = prizeBanner.jackpot;
            prizeBanner.jackpot = null;
            lostFiftyFifty = false;
            return prize;
        }
        
        if (Prefs.DevMode)
        {
            Log.Message("Lost 50/50. Generating new jackpot item:");
        }

        lostFiftyFifty = true;
        return prizeBanner.GeneratePrize(PrizeCategory.Jackpot, prizeBanner.PityThreshold/settings.pullPriceFactor, true);
    }

    public Thing SelectConsolation(PrizeBanner prizeBanner)
    {
        if (Rand.Chance(settings.getConsolationFeatured) && !prizeBanner.consolationPrizes.NullOrEmpty())
        {
            if (Prefs.DevMode)
            {
                // Log.Message("Won 50/50, deploying new item:");
            }

            var prize = prizeBanner.consolationPrizes.RandomElement();
            prizeBanner.consolationPrizes.Remove(prize);
            return prize;
        }
        
        if (Prefs.DevMode)
        {
            // Log.Message("Won 50/50, deploying new item:");
        }
        return prizeBanner.GeneratePrize(PrizeCategory.Consolation,
            prizeBanner.PityThreshold / settings.pullPriceFactor * settings.consolationChance);
    }

    public GameComponent_GachaTracker(): base()
    {
    }
    public GameComponent_GachaTracker(Game game)
    {
    }
}