using System.Linq;
using ArchoGacha.Utils;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeBannerClasses;

public class EquipmentPrizeBanner : PrizeBanner
{
    public override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.one,
            filter = PrizeFilter,
            validator = ReqValidator
        };

        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        var thingStuffPairs =
            ArchoGachaUtils.CalculateAllowedThingStuffPairs(def, allowedDefs,
                prizeCategory, valueMaxOverride).ToList();

        return !thingStuffPairs.NullOrEmpty()
            ? thingStuffPairs.RandomElement().MakeThing()
            : null;
    }
}