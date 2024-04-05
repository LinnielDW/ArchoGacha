﻿using System;
using System.Collections.Generic;
using ArchoGacha.Settings;
using RimWorld;
using Verse;

namespace ArchoGacha.Utils;

public static class ArchoGachaUtils
{
    private static readonly Func<PrizeCategory, QualityGenerator>
        QualityFromPrizeCat = prizeCategory =>
            prizeCategory == PrizeCategory.Jackpot
                ? QualityGenerator.Super
                : QualityGenerator.Reward;

    public static IEnumerable<ThingStuffPairWithQuality>
        CalculateAllowedThingStuffPairs(IEnumerable<ThingDef> allowed,
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

                    if (IsValidStuffPair(prizeCategory, stuffPair,
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

    private static bool IsValidStuffPair(PrizeCategory prizeCategory,
        ThingStuffPairWithQuality stuffPair, float valueMaxOverride = 0f)
    {
        var marketValue = stuffPair.GetStatValue(StatDefOf.MarketValue);
        if (prizeCategory == PrizeCategory.Jackpot)
        {
            return marketValue > ArchoGachaSettings.minJackpotOffset;
        }

        return (valueMaxOverride == 0f &&
                marketValue >= ArchoGachaSettings.minConsolationOffset) ||
               (valueMaxOverride != 0f && marketValue <= valueMaxOverride
                   // +ArchoGachaSettings.maxConsolationOffset
               );
    }
}