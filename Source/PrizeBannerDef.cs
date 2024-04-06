using System;
using System.Collections.Generic;
using ArchoGacha.GameComps;
using RimWorld;
using Verse;

namespace ArchoGacha;

public class PrizeBannerDef : Def
{
    public Type prizeBannerClass = typeof(PrizeBanner);
    public IntRange spawnCountRange = IntRange.one;
    public IntRange consolationCountRange = new(2, 3);
    public ThingCategoryDef thingCategoryDef = ThingCategoryDefOf.Items;
    public List<ThingCategoryDef> excludedCategoryDefs = new();
    public TechLevel minTechLevel = TechLevel.Undefined;

    //TODO: change price logic to be if(banner min > settingsMin) use settings min 
    public float minJackpotMarketValue;
    public float minConsolationMarketValue;
    
}