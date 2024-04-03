using System.Linq;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public class QualityWeaponPrizeWorker : PrizeWorker
{
    public override Thing GeneratePrize(PrizeCategory prizeCategory)
    {
        var prizeDef = SelectPrizeDef(prizeCategory);

        ThingDef stuffDef = null;
        if (prizeDef.MadeFromStuff)
        {
            stuffDef = GenStuff.AllowedStuffsFor(prizeDef).Where(stuff =>
                    PrizeMarketValue(StatWorker_MarketValue.CalculatedBaseMarketValue(prizeDef, stuff), prizeCategory))
                .RandomElement();
        }

        var prize = ThingMaker.MakeThing(prizeDef, stuffDef);

        prize.TryGetComp<CompQuality>()?.SetQuality(
            QualityUtility.GenerateQuality(prizeCategory == PrizeCategory.Jackpot
                ? QualityGenerator.Super
                : QualityGenerator.Reward),
            ArtGenerationContext.Colony);

        return prize;
    }

    public override ThingDef SelectPrizeDef(PrizeCategory prizeCategory)
    {
        return DefDatabase<ThingDef>.AllDefsListForReading
            .Where(t => t.thingCategories.Contains(ThingCategoryDefOf.Weapons) &&
                        (
                            (t.techLevel >= TechLevel.Spacer && t.HasComp<CompQuality>()) ||
                            (t.MadeFromStuff && GenStuff.AllowedStuffsFor(t).Any(stuff =>
                                PrizeMarketValue(StatWorker_MarketValue.CalculatedBaseMarketValue(t, stuff),
                                    prizeCategory)))
                        )).RandomElement();
    }
}