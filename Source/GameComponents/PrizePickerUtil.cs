using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Defs;
using ArchoGacha.Utils;

namespace ArchoGacha.GameComponents;

public static class PrizePickerUtil
{
    public static Thing DecidePrize(this GameComponent_GachaTracker gachaTracker, PrizeBanner prizeBanner)
    {
        var randOutcome = Rand.Value;
        if (Prefs.DevMode)
        {
            // Log.Message($"Rand outcome: {randOutcome}");
        }

        switch (randOutcome)
        {
            case var _ when randOutcome < settings.jackpotChance || gachaTracker.pitySilverReserve >= prizeBanner.PityThreshold:
            {
                // pityCount = 0;
                var jackpot = gachaTracker.SelectJackpot(prizeBanner);
                gachaTracker.pitySilverReserve = Math.Max(0, gachaTracker.pitySilverReserve - (int)(jackpot.MarketValue * jackpot.stackCount * settings.pullPriceFactor * jackpot.stackCount));
                Find.LetterStack.ReceiveLetter("ArchoGacha_JackpotLetterLabel".Translate(), "ArchoGacha_JackpotLetter".Translate(jackpot.LabelShortCap),ArchoGachaDefOf.ArchoGacha_Jackpot, jackpot, hyperlinkThingDefs: new List<ThingDef>() {jackpot.def});
                return jackpot;
            }
            case var _ when randOutcome < settings.consolationChance:
            {
                // pityCount = 0;
                var consolation = SelectConsolation(prizeBanner);
                if (consolation != null)
                {
                    Messages.Message("ArchoGacha_Consolation".Translate(consolation.LabelShortCap),consolation, MessageTypeDefOf.PositiveEvent);
                    return consolation;
                }
                goto default;
            }
            default:
            {
                var trash = gachaTracker.trashGeneric;
                if (!prizeBanner.def.trashOverrideDefs.NullOrEmpty())
                {
                    trash = prizeBanner.def.trashOverrideDefs;
                }

                var trashFiltered = trash
                    .Where(t => t.BaseMarketValue <= prizeBanner.pullPrice/settings.pullPriceFactor)
                    .ToList();
                var trashDef = trashFiltered.NullOrEmpty()
                    ? ThingDefOf.WoodLog
                    : trashFiltered.RandomElement();
                return ThingMaker.MakeThing(trashDef, trashDef.defaultStuff);
            }
        }
    }

    public static Thing SelectJackpot(this GameComponent_GachaTracker gachaTracker, PrizeBanner prizeBanner)
    {
        if ((Rand.Chance(settings.getFeatured) || gachaTracker.lostFiftyFifty) && prizeBanner.jackpot != null)
        {
            if (Prefs.DevMode)
            {
                Log.Message("Won 50/50, deploying jackpot item:");
            }

            var prize = prizeBanner.jackpot;
            prizeBanner.jackpot = null;
            gachaTracker.lostFiftyFifty = false;
            return prize;
        }
        
        if (Prefs.DevMode)
        {
            Log.Message("Lost 50/50. Generating new jackpot item:");
        }

        gachaTracker.lostFiftyFifty = true;
        return prizeBanner.GeneratePrize(PrizeCategory.Jackpot, prizeBanner.PityThreshold/settings.pullPriceFactor, true);
    }

    public static Thing SelectConsolation(PrizeBanner prizeBanner)
    {
        if (Rand.Chance(settings.getConsolationFeatured) && !prizeBanner.consolationPrizes.NullOrEmpty())
        {
            if (Prefs.DevMode)
            {
                // Log.Message("Won 50/50, deploying new item:");
            }

            var prize = prizeBanner.consolationPrizes.RandomElement();
            prizeBanner.consolationPrizes.Remove(prize);
            return prize;
        }
        
        if (Prefs.DevMode)
        {
            // Log.Message("Won 50/50, deploying new item:");
        }
        return prizeBanner.GeneratePrize(PrizeCategory.Consolation,
            prizeBanner.PityThreshold / settings.pullPriceFactor * settings.consolationChance);
    }
}