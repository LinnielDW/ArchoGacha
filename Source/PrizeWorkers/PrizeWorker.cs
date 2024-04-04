using Verse;

namespace ArchoGacha;

public abstract class PrizeWorker
{
    public PrizeGeneratorDef def;

    public abstract Thing GeneratePrize(PrizeCategory prizeCategory);

    public abstract Thing SelectPrizeDef(PrizeCategory prizeCategory);
}

public enum PrizeCategory
{
    Jackpot,
    Consolation
}