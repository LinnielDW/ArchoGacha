using System.Linq;
using ArchoGacha.GameComps;
using ArchoGacha.Utils;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeBanners;

public class ArmorPrizeBanner : PrizeBanner
{
    // protected override ThingCategoryDef FilterCategory =>
    //     ThingCategoryDefOf.ApparelArmor;

    public override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f)
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
                prizeCategory, valueMaxOverride).ToList();

        return !thingStuffPairs.NullOrEmpty()
            ? thingStuffPairs.RandomElement().MakeThing()
            : null;
    }

    public override bool ReqValidator(ThingDef thingDef)
    {
        return !thingDef.destroyOnDrop &&
               thingDef.techLevel >= MinTechLevel;
    }
}