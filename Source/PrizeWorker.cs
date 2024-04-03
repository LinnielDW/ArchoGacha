using System;
using RimWorld;
using Verse;

namespace ArchoGacha;

public abstract class PrizeWorker
{
    public PrizeGeneratorDef def;

    public virtual Thing GeneratePrize(PrizeCategory prizeCategory)
    {
        throw new NotImplementedException();
    }

    public bool PrizeMarketValue(ThingStuffPair stuffPair, PrizeCategory prizeCategory)
    {
        var marketValue = stuffPair.Price;
        if (stuffPair.thing.HasComp<CompQuality>())
        {
            //TODO: Move this to a setting
            marketValue = prizeCategory == PrizeCategory.Jackpot
                ? marketValue * 2.5f + 1500
                : marketValue * 1.5f + 500;
        }

        if (prizeCategory == PrizeCategory.Jackpot) return marketValue >= def.minJackpotMarketValue;

        return marketValue >= def.minConsolationMarketValue;
    }

    public virtual ThingStuffPair SelectPrizeDef(PrizeCategory prizeCategory)
    {
        throw new NotImplementedException();
    }
}

public enum PrizeCategory
{
    Jackpot,
    Consolation
}

public static class PrizeWorkerUtils
{
}