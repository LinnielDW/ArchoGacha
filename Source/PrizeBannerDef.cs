using System.Collections.Generic;

namespace ArchoGacha;

public class PrizeBannerDef : Def
{
    public Type prizeBannerClass = typeof(PrizeBanner);
    public IntRange spawnCountRange = IntRange.one;
    public IntRange consolationCountRange = new(2, 3);
    public ThingCategoryDef thingCategoryDef;
    public List<ThingCategoryDef> excludedCategoryDefs = new();
    public TechLevel minTechLevel = TechLevel.Undefined;

    public float minJackpotMarketValue;
    
    //the minimum value something can spawn when the settings.useGlobalConsolationOffset set to true
    public float minConsolationMarketValue = 100f;
    
}