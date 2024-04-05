using System;
using System.Collections.Generic;
using ArchoGacha.Utils;
using RimWorld;
using Verse;

namespace ArchoGacha.GameComps;

public class PrizeBanner : IExposable
{
    public PrizeBannerDef def;
    public Thing jackpot;
    public List<Thing> prizes;

    public PrizeBanner()
    {
    }

    public PrizeBanner(PrizeBannerDef def, Thing jackpot, List<Thing> prizes)
    {
        this.jackpot = jackpot;
        this.prizes = prizes;
        this.def = def;
    }

    public PrizeBanner(PrizeBannerDef def)
    {
        this.def = def;
    }

    public override string ToString()
    {
        return
            $"{def.LabelCap} {{ jackpot={jackpot}, prizes={string.Join(",", prizes)}, def={def} }}";
    }

    public void ExposeData()
    {
        Scribe_Defs.Look(ref def, "def");
        Scribe_Deep.Look(ref jackpot, true, "jackpot");
        Scribe_Collections.Look(ref prizes, "prizes", true, LookMode.Deep);
    }

    protected virtual ThingCategoryDef FilterCategory => def.thingCategoryDef;

    protected virtual TechLevel MinTechLevel => def.minTechLevel;

    public virtual void GenerateJackpot(PrizeCategory prizeCategory)
    {
        jackpot = GeneratePrize(prizeCategory);
    }

    public virtual Thing GenerateConsolationThing(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f)
    {
        return GeneratePrize(prizeCategory, valueMaxOverride);
    }

    public virtual Thing GeneratePrize(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f)
    {
        var prize = SelectPrizeDef(prizeCategory, valueMaxOverride);
        if (prize == null)
        {
            Log.Warning(
                $"{def.LabelCap} attempted to generate a prize but could not! Consider tweaking the minimum prize thresholds to a lower value");
        }

        return prize;
    }

    public virtual Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0f)
    {
        throw new NotImplementedException();
    }

    public virtual bool ReqValidator(ThingDef thingDef)
    {
        throw new NotImplementedException();
    }
}