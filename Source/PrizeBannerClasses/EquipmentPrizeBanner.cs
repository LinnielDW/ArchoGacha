using System.Linq;
using ArchoGacha.Utils;

namespace ArchoGacha.PrizeBannerClasses;

public class EquipmentPrizeBanner : PrizeBanner
{
    protected override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f, bool excludeJackpotDef = false)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.One,
            filter = PrizeFilter,
            validator = ReqValidator
        };

        if (excludeJackpotDef && jackpot != null)
        {
            req.filter.SetAllow(jackpot.def, false);
        }

        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        var thingStuffPairs =
            ArchoGachaUtils.CalculateAllowedThingStuffPairs(def, allowedDefs,
                prizeCategory, valueMaxOverride).ToList();

        return !thingStuffPairs.NullOrEmpty()
            ? thingStuffPairs.RandomElement().MakeThing()
            : null;
    }
}