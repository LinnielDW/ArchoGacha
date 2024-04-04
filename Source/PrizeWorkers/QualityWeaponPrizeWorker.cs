using System.Linq;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public class QualityWeaponPrizeWorker : PrizeWorker
{
    protected virtual ThingCategoryDef FilterCategory =>
        ThingCategoryDefOf.Weapons;

    protected virtual TechLevel MinTechLevel => TechLevel.Industrial;

    public override Thing GeneratePrize(PrizeCategory prizeCategory)
    {
        var prize = SelectPrizeDef(prizeCategory);
        if (prize == null)
        {
            Log.Error(
                $"{nameof(QualityWeaponPrizeWorker)} attempted to generate a prize but could not! Consider tweaking the minimum prize thresholds to a lower value");
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

        req.filter.SetAllow(FilterCategory, true);
        req.validator = x =>
            x.IsWeapon && !x.destroyOnDrop &&
            x.techLevel >= MinTechLevel;

        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        var thingStuffPairs =
            ArchoGachaUtils.CalculateAllowedThingStuffPairs(allowedDefs,
                prizeCategory).ToList();

        if (!thingStuffPairs.NullOrEmpty())
            return thingStuffPairs.RandomElement().MakeThing();
        return null;
    }
}

public class RangedWeaponPrizeWorker : QualityWeaponPrizeWorker
{
    protected override ThingCategoryDef FilterCategory =>
        ArchoGachaDefOf.WeaponsRanged;
}

public class MeleeWeaponPrizeWorker : QualityWeaponPrizeWorker
{
    protected override ThingCategoryDef FilterCategory =>
        ArchoGachaDefOf.WeaponsMelee;

    protected override TechLevel MinTechLevel => TechLevel.Medieval;
}