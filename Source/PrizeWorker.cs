using System;
using RimWorld;
using Verse;
using static ArchoGacha.ArchoGachaSettings;

namespace ArchoGacha;

public abstract class PrizeWorker
{
    public PrizeGeneratorDef def;

    public abstract Thing GeneratePrize(PrizeCategory prizeCategory);

    public bool PrizeMarketValue(ThingStuffPair stuffPair, PrizeCategory prizeCategory)
    {
        var marketValue = stuffPair.Price;
        if (stuffPair.thing.HasComp<CompQuality>())
        {
            marketValue = prizeCategory == PrizeCategory.Jackpot
                ? marketValue * minJackpotFactor + minJackpotOffset
                : marketValue * minConsolationFactor + minConsolationOffset;
        }

        if (prizeCategory == PrizeCategory.Jackpot) return marketValue >= def.minJackpotMarketValue;

        return marketValue >= def.minConsolationMarketValue;
    }

    public abstract ThingStuffPair SelectPrizeDef(PrizeCategory prizeCategory);
}

public enum PrizeCategory
{
    Jackpot,
    Consolation
}

public static class PrizeWorkerUtils
{
}