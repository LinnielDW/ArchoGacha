using Verse;

namespace ArchoGacha;

public abstract class PrizeWorker
{
    public PrizeGeneratorDef def;
    public abstract Thing GeneratePrize(PrizeCategory prizeCategory);

    public bool PrizeMarketValue(float baseMarketValue, PrizeCategory prizeCategory)
    {
        if (prizeCategory == PrizeCategory.Jackpot) return baseMarketValue * 2.5f + 1500 >= def.minJackpotMarketValue;

        return baseMarketValue * 1.5f + 500 >= def.minConsolationMarketValue;
    }

    public abstract ThingDef SelectPrizeDef(PrizeCategory prizeCategory);
}

public enum PrizeCategory
{
    Jackpot,
    Consolation
}

public static class PrizeWorkerUtils
{
}