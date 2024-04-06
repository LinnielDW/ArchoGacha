using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using static ArchoGacha.ArchoGachaMod;

namespace ArchoGacha.Utils;

public static class ArchoGachaUtils
{
    private static readonly Func<PrizeCategory, QualityGenerator>
        QualityFromPrizeCat = prizeCategory =>
            prizeCategory == PrizeCategory.Jackpot
                ? QualityGenerator.Super
                : QualityGenerator.Reward;

    //TODO: consider moving this to EquipmentPrizeBanner
    public static IEnumerable<ThingStuffPairWithQuality>
        CalculateAllowedThingStuffPairs(PrizeBannerDef bannerDef,IEnumerable<ThingDef> allowed,
            PrizeCategory prizeCategory, float valueMaxOverride = 0f)
    {
        var qualityGenerator = QualityFromPrizeCat(prizeCategory);
        foreach (var thingDef in allowed)
        {
            for (var i = 0; i < 5; i++)
            {
                if (GenStuff.TryRandomStuffFor(thingDef, out var stuff,
                        validator: IsDerpValidator))
                {
                    var quality = GenerateQuality(qualityGenerator, thingDef);
                    var stuffPair =
                        new ThingStuffPairWithQuality(thingDef, stuff, quality);

                    if (IsValidStuffPair(bannerDef, prizeCategory, stuffPair,
                            valueMaxOverride))
                    {
                        yield return stuffPair;
                    }
                }
            }

            continue;

            bool IsDerpValidator(ThingDef stuffDef) =>
                !ThingSetMakerUtility.IsDerpAndDisallowed(thingDef, stuffDef,
                    qualityGenerator);
        }
    }

    private static QualityCategory GenerateQuality(
        QualityGenerator qualityGenerator, ThingDef thingDef)
    {
        return thingDef.HasComp(typeof(CompQuality))
            ? QualityUtility.GenerateQuality(qualityGenerator)
            : QualityCategory.Normal;
    }

    private static bool IsValidStuffPair(PrizeBannerDef bannerDef, PrizeCategory prizeCategory,
        ThingStuffPairWithQuality stuffPair, float valueMaxOverride = 0f)
    {
        var marketValue = stuffPair.GetStatValue(StatDefOf.MarketValue);
        switch (prizeCategory)
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
        }
    }
}