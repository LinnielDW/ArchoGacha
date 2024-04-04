using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public static class ArchoGachaUtils
{
    private static readonly Func<PrizeCategory, QualityGenerator> QualityFromPrizeCat = prizeCategory =>  prizeCategory == PrizeCategory.Jackpot ? QualityGenerator.Super : QualityGenerator.Reward;
    
    public static IEnumerable<ThingStuffPairWithQuality> CalculateAllowedThingStuffPairs(IEnumerable<ThingDef> allowed, PrizeCategory prizeCategory)
    {
        var qualityGenerator = QualityFromPrizeCat(prizeCategory);
        foreach (var thingDef in allowed)
        {
            for (var i = 0; i < 5; i++)
            {
                if (GenStuff.TryRandomStuffFor(thingDef, out var stuff, validator: IsDerpValidator))
                {
                    var quality = GenerateQuality(qualityGenerator, thingDef);
                    var stuffPair = new ThingStuffPairWithQuality(thingDef, stuff, quality);
                    
                    if (IsValidStuffPair(prizeCategory, stuffPair))
                    {
                        yield return stuffPair;
                    }
                }
            }

            continue;

            bool IsDerpValidator(ThingDef stuffDef) => !ThingSetMakerUtility.IsDerpAndDisallowed(thingDef, stuffDef, qualityGenerator);
        }
    }

    private static QualityCategory GenerateQuality(QualityGenerator qualityGenerator, ThingDef thingDef)
    {
        return thingDef.HasComp(typeof(CompQuality)) ? QualityUtility.GenerateQuality(qualityGenerator) : QualityCategory.Normal;
    }

    private static bool IsValidStuffPair(PrizeCategory prizeCategory, ThingStuffPairWithQuality stuffPair)
    {
        var marketValue = stuffPair.GetStatValue(StatDefOf.MarketValue);
        if (prizeCategory == PrizeCategory.Jackpot)
        {
            return  marketValue > ArchoGachaSettings.minJackpotOffset;
        }
        return marketValue > ArchoGachaSettings.minConsolationOffset && marketValue < ArchoGachaSettings.minJackpotOffset + ArchoGachaSettings.minConsolationOffset * 0.5f;
    }
}