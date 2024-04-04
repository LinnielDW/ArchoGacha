using RimWorld;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public class QualityWeaponPrizeWorker : PrizeWorker
{
    public override Thing GeneratePrize(PrizeCategory prizeCategory)
    {
        var prize = SelectPrizeDef(prizeCategory);
        if (prize == null)
        {
            Log.Error($"{nameof(QualityWeaponPrizeWorker)} attempted to generate a prize but could not! Consider tweaking the minimum prize thresholds to a lower value");
        }

        return prize;
    }

    public override Thing SelectPrizeDef(PrizeCategory prizeCategory)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.one,
            filter = new ThingFilter()
        };

        req.filter.SetAllow(ThingCategoryDefOf.Weapons, true);
        req.validator = x => x.IsWeapon && !x.destroyOnDrop && x.techLevel >= TechLevel.Industrial;
        
        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        var thingStuffPairs = ArchoGachaUtils.CalculateAllowedThingStuffPairs(allowedDefs, prizeCategory);
        return thingStuffPairs.RandomElement().MakeThing();
    }
}