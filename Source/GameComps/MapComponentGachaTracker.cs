using System;
using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Utils;
using RimWorld;
using Verse;
using static ArchoGacha.ArchoGachaMod;

namespace ArchoGacha.GameComps;

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
        prizeBanner.GenerateJackpot(PrizeCategory.Jackpot);
        prizeBanner.prizes = GenerateConsolations(prizeBanner,
            //TODO: turn multiplier into a setting
            prizeBanner.jackpot.MarketValue * 0.2f).ToList();

        return prizeBanner;
    }

    public static IEnumerable<Thing> GenerateConsolations(
        PrizeBanner prizeBanner,
        float valueMaxOverride = 0f)
    {
        for (int i = 0; i < prizeBanner.def.consolationCountRange.RandomInRange; i++)
        {
            var prize =
                prizeBanner.GenerateConsolationThing(PrizeCategory.Consolation,
                    valueMaxOverride);
            if (prize != null) yield return prize;
        }
    }

    public void PullOnBanner(PrizeBanner prizeBanner)
    {
        //TODO: impl
        var amount = (int)(prizeBanner.jackpot.MarketValue *
                           settings.jackpotChance);
        if (TradeUtility.ColonyHasEnoughSilver(map, amount))
        {
            TradeUtility.LaunchSilver(map, amount);
        }
    }

    private Thing DecidePrize()
    {
        throw new NotImplementedException();
    }

    public MapComponentGachaTracker(Map map) : base(map)
    {
    }
}