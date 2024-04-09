using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Utils;

namespace ArchoGacha.PrizeBannerClasses;

//TODO: consider: don't need?
public class BionicPrizeBanner : PrizeBanner
{
    public override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0F, bool excludeJackpotDef = false)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.one,
            filter = PrizeFilter,
            validator = ReqValidator,
        };

        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        var thingStuffPairs =
            CalculateAllowedThingStuffPairs(def, allowedDefs,
                prizeCategory, valueMaxOverride).ToList();

        var x = thingStuffPairs.RandomElement();
        
        var t = ThingMaker.MakeThing(x.thing, x.stuff);
        if (!ArchoGachaUtils.IsValidStuffPair(def, prizeCategory, x,
                valueMaxOverride))
        {
            if (prizeCategory == PrizeCategory.Jackpot)
            {
                if (valueMaxOverride != 0f && t.MarketValue < valueMaxOverride)
                {
                    t.stackCount = (int)(valueMaxOverride / t.MarketValue);
                }
                else if (valueMaxOverride == 0f && 
                         t.MarketValue < Math.Max(settings.minJackpotOffset, def.minJackpotMarketValue))
                {
                    t.stackCount = (int)(Math.Max(settings.minJackpotOffset,
                                             def.minJackpotMarketValue) /
                                         t.MarketValue);
                }
            }
            else
            {
                if (settings.useGlobalConsolationOffset &&
                    t.MarketValue < Math.Max(settings.minConsolationOffset,
                        def.minConsolationMarketValue))
                {
                    t.stackCount = (int)(Math.Max(
                        settings.minConsolationOffset,
                        def.minConsolationMarketValue) / t.MarketValue);
                }
                else if(valueMaxOverride != 0f && t.MarketValue < valueMaxOverride)
                {
                    t.stackCount = (int)(valueMaxOverride / t.MarketValue);
                }
            }


        }


        return t;
        // var t = new ThingSetMaker_StackCount().Generate(req);



        /*switch (prizeCategory)
        {
            case PrizeCategory.Jackpot:
            {
                return (valueMaxOverride == 0f &&
                        marketValue >= Math.Max(settings.minJackpotOffset, bannerDef.minJackpotMarketValue)) ||
                       valueMaxOverride != 0f && marketValue <= valueMaxOverride;
            }
            case PrizeCategory.Consolation:
            default:
            {
                return (settings.useGlobalConsolationOffset &&
                        marketValue >= Math.Max(settings.minConsolationOffset, bannerDef.minConsolationMarketValue) &&
                        marketValue <= settings.minJackpotOffset) ||
                       (valueMaxOverride != 0f && marketValue <= valueMaxOverride);
            }
        }*/
    }
    
    public static IEnumerable<ThingStuffPairWithQuality>
        CalculateAllowedThingStuffPairs(PrizeBannerDef bannerDef,IEnumerable<ThingDef> allowed,
            PrizeCategory prizeCategory, float valueMaxOverride = 0f)
    {
        var qualityGenerator = ArchoGachaUtils.QualityFromPrizeCat(prizeCategory);
        foreach (var thingDef in allowed)
        {
            for (var i = 0; i < 5; i++)
            {
                if (GenStuff.TryRandomStuffFor(thingDef, out var stuff,
                        validator: IsDerpValidator))
                {
                    var quality = ArchoGachaUtils.GenerateQuality(qualityGenerator, thingDef);
                    var stuffPair =
                        new ThingStuffPairWithQuality(thingDef, stuff, quality);

                    
                    yield return stuffPair;
                }
                
            }

            continue;

            bool IsDerpValidator(ThingDef stuffDef) =>
                !ThingSetMakerUtility.IsDerpAndDisallowed(thingDef, stuffDef,
                    qualityGenerator);
        }
    }
}