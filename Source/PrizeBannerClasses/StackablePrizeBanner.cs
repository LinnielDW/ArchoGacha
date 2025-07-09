using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Utils;

namespace ArchoGacha.PrizeBannerClasses;

public class StackablePrizeBanner : PrizeBanner
{
    protected override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0F, bool excludeJackpotDef = false)
    {
        var req = new ThingSetMakerParams
        {
            countRange = IntRange.One,
            filter = PrizeFilter,
            validator = ReqValidator,
        };

        var allowedDefs = ThingSetMakerUtility.GetAllowedThingDefs(req);
        if (prizeCategory == PrizeCategory.Consolation)
        {
            allowedDefs = allowedDefs.Where(t =>valueMaxOverride != 0f &&
                                            t.BaseMarketValue *
                                            def.valueMultiplier <=
                                            valueMaxOverride * 0.75f);
        }
        
        var thingStuffPairs = CalculateAllowedThingStuffPairs(allowedDefs, prizeCategory).ToList();

        var x = thingStuffPairs.RandomElement();
        
        var t = ThingMaker.MakeThing(x.thing, x.stuff);
        //too expensive
        
        var marketValue = t.MarketValue * t.stackCount;

        if (!IsAttractiveEnough(def, prizeCategory, t, valueMaxOverride))
        {
            if (prizeCategory == PrizeCategory.Jackpot)
            {
                if (valueMaxOverride != 0f && marketValue < valueMaxOverride)
                {
                    t.stackCount = (int)Math.Ceiling(valueMaxOverride / marketValue);
                }
                else if (valueMaxOverride == 0f && 
                         t.MarketValue < Math.Max(settings.minJackpotOffset, def.minJackpotMarketValue))
                {
                    t.stackCount = (int)Math.Ceiling(Math.Max(settings.minJackpotOffset, def.minJackpotMarketValue) / marketValue);
                }
            }
            else
            {
                if (settings.useGlobalConsolationOffset &&
                    t.MarketValue < Math.Max(settings.minConsolationOffset,
                        def.minConsolationMarketValue))
                {
                    t.stackCount = (int)Math.Ceiling(Math.Max(
                        settings.minConsolationOffset,
                        def.minConsolationMarketValue) / marketValue);
                }
                else if(valueMaxOverride != 0f && marketValue < valueMaxOverride)
                {
                    t.stackCount = (int)Math.Ceiling(valueMaxOverride / marketValue);
                }
            }
        }

        return t;
    }
    
    public static IEnumerable<ThingStuffPairWithQuality>
        CalculateAllowedThingStuffPairs(IEnumerable<ThingDef> allowed,
            PrizeCategory prizeCategory)
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
    
    public static bool IsAttractiveEnough(PrizeBannerDef bannerDef,
        PrizeCategory prizeCategory,
        Thing t, float valueMaxOverride = 0f)
    {
        var marketValue = t.MarketValue * bannerDef.valueMultiplier;
        switch (prizeCategory)
        {
            case PrizeCategory.Jackpot:
            {
                return (valueMaxOverride == 0f && 
                        marketValue >= Math.Max(settings.minJackpotOffset, bannerDef.minJackpotMarketValue)) ||
                       valueMaxOverride != 0f && marketValue <= valueMaxOverride && marketValue >= valueMaxOverride * 0.75f;
            }
            case PrizeCategory.Consolation:
            default:
            {
                return (settings.useGlobalConsolationOffset && 
                        marketValue >= Math.Max(settings.minConsolationOffset, bannerDef.minConsolationMarketValue) && 
                        marketValue <= settings.minJackpotOffset) || 
                       (valueMaxOverride != 0f && marketValue > valueMaxOverride * 0.5f);
            }
        }
    }
}