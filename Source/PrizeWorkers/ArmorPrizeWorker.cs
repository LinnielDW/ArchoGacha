using System.Linq;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public class ArmorPrizeWorker : PrizeWorker
{
    protected override ThingCategoryDef FilterCategory =>
        ThingCategoryDefOf.ApparelArmor;

    public override Thing SelectPrizeDef(PrizeCategory prizeCategory)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.one,
            filter = new ThingFilter()
        };

        req.filter.SetAllow(FilterCategory, true);
        req.validator = ReqValidator;

        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        var thingStuffPairs =
            ArchoGachaUtils.CalculateAllowedThingStuffPairs(allowedDefs,
                prizeCategory).ToList();

        return !thingStuffPairs.NullOrEmpty()
            ? thingStuffPairs.RandomElement().MakeThing()
            : null;
    }

    public override bool ReqValidator(ThingDef thingDef)
    {
        return thingDef.IsApparel && !thingDef.destroyOnDrop &&
               thingDef.techLevel >= MinTechLevel;
    }
}