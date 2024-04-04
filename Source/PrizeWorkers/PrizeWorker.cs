using RimWorld;
using Verse;

namespace ArchoGacha;

public abstract class PrizeWorker
{
    public PrizeGeneratorDef def;


    protected virtual ThingCategoryDef FilterCategory { get; }

    protected virtual TechLevel MinTechLevel => TechLevel.Industrial;

    public virtual Thing GeneratePrize(PrizeCategory prizeCategory)
    {
        var prize = SelectPrizeDef(prizeCategory);
        if (prize == null)
        {
            Log.Error(
                $"{def.LabelCap} attempted to generate a prize but could not! Consider tweaking the minimum prize thresholds to a lower value");
        }

        return prize;
    }

    public abstract Thing SelectPrizeDef(PrizeCategory prizeCategory);
    public abstract bool ReqValidator(ThingDef thingDef);
}

public enum PrizeCategory
{
    Jackpot,
    Consolation
}