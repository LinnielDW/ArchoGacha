﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using ArchoGacha.Utils;

namespace ArchoGacha;

public abstract class PrizeBanner : IExposable
{
    public PrizeBannerDef def;
    public Thing jackpot;
    public List<Thing> consolationPrizes = new List<Thing>();
    public int pullPrice;

    private int pityThresholdInt = -1;
    
    public int PityThreshold
    {
        get
        {
            if (pityThresholdInt < 0)
            {
                pityThresholdInt = (int)Math.Ceiling(pullPrice / settings.jackpotChance);
            }

            return pityThresholdInt;
        }
    }

    public PrizeBanner()
    {
    }

    public PrizeBanner(PrizeBannerDef def, Thing jackpot, List<Thing> consolationPrizes)
    {
        this.jackpot = jackpot;
        this.consolationPrizes = consolationPrizes;
        this.def = def;
    }

    public PrizeBanner(PrizeBannerDef def)
    {
        this.def = def;
    }

    public override string ToString()
    {
        return
            $"{def.LabelCap} {{ jackpot={jackpot}, consolationPrizes={string.Join(",", consolationPrizes)}, def={def} }}";
    }

    public void ExposeData()
    {
        Scribe_Defs.Look(ref def, "def");
        Scribe_Deep.Look(ref jackpot, true, "jackpot");
        Scribe_Collections.Look(ref consolationPrizes, "consolationPrizes", true, LookMode.Deep);
        Scribe_Values.Look(ref pullPrice, "pullPrice");
    }

    protected virtual ThingFilter PrizeFilter
    {
        get
        {
            if (filterInt == null || filterInt.AllowedDefCount == 0)
            {
                filterInt = new ThingFilter();

                // filterInt.SetAllow(def.includeCategoryDefs, true);
                foreach (var inclusion in def.includeCategoryDefs)
                {
                    filterInt.SetAllow(inclusion, true);
                }
                foreach (var exclusion in def.excludedCategoryDefs)
                {
                    filterInt.SetAllow(exclusion, false);
                }


                if (filterInt.AllowedDefCount == 0)
                {
                    Log.Error($"{def.LabelCap} could not find anything matching its defined filter properties");
                }
            }

            return filterInt;
        }
    }

    private ThingFilter filterInt;

    protected virtual TechLevel MinTechLevel => def.minTechLevel;

    public virtual void GenerateJackpot()
    {
        var generatedJackpot = GeneratePrize(PrizeCategory.Jackpot);
        jackpot = generatedJackpot;
        CalculatePullPrice();
    }

    public virtual void CalculatePullPrice()
    {
        pullPrice = (int)Math.Ceiling(jackpot.MarketValue * 
                                           settings.jackpotChance *
                                           settings.pullPriceFactor *
                                           jackpot.stackCount);
    }

    public virtual Thing GenerateConsolationThing(float valueMaxOverride = 0f)
    {
        return GeneratePrize(PrizeCategory.Consolation, valueMaxOverride);
    }

    public virtual Thing GeneratePrize(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f, bool excludeJackpotDef = false)
    {
        var prize = SelectPrizeDef(prizeCategory, valueMaxOverride, excludeJackpotDef);
        if (prize == null)
        {
            // Log.Warning($"ArchoGacha {def.LabelCap} attempted to generate a prize but could not! Skipping...");
        }

        return prize;
    }

    public bool CanPull(Map map)
    {
        return TradeUtility.ColonyHasEnoughSilver(map, pullPrice);
    }    
    
    public bool CanPullTen(Map map)
    {
        return TradeUtility.ColonyHasEnoughSilver(map, pullPrice * 10);
    }

    protected abstract Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0F, bool excludeJackpotDef = false);

    protected virtual bool ReqValidator(ThingDef thingDef)
    {
        return !thingDef.destroyOnDrop &&
               thingDef.techLevel >= MinTechLevel && !consolationPrizes.Any(m => m.def == thingDef);
    }
}