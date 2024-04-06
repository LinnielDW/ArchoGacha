using System.Linq;
using ArchoGacha.GameComps;
using ArchoGacha.Utils;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeBanners;

public class EquipmentPrizeBanner : PrizeBanner
{
    public override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.one,
            filter = PrizeFilterInt,
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