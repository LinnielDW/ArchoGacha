using System.Linq;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public class QualityWeaponPrizeWorker : PrizeWorker
{
    public override Thing GeneratePrize(PrizeCategory prizeCategory)
    {
        var stuffPair = SelectPrizeDef(prizeCategory);
        var prize = ThingMaker.MakeThing(stuffPair.thing, stuffPair.stuff);

        prize.TryGetComp<CompQuality>()?.SetQuality(
            QualityUtility.GenerateQuality(prizeCategory == PrizeCategory.Jackpot
                ? QualityGenerator.Super
                : QualityGenerator.Reward),
            ArtGenerationContext.Colony);

        return prize;
    }

    public override ThingStuffPair SelectPrizeDef(PrizeCategory prizeCategory)
    {
        return PawnWeaponGenerator.allWeaponPairs.Where(stuffPair =>
            PrizeMarketValue(stuffPair, prizeCategory) && stuffPair.thing.techLevel >= TechLevel.Industrial &&
            ThingSetMakerUtility.CanGenerate(stuffPair.thing)).RandomElement();
    }
}