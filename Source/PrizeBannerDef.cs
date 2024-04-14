using System.Collections.Generic;

namespace ArchoGacha;

public class PrizeBannerDef : Def
{
    public Type prizeBannerClass = typeof(PrizeBanner);
    public IntRange spawnCountRange = IntRange.one;
    public IntRange consolationCountRange = new(2, 3);
    public List<ThingCategoryDef> includeCategoryDefs = new();
    public List<ThingCategoryDef> excludedCategoryDefs = new();
    public TechLevel minTechLevel = TechLevel.Undefined;
    public float valueMultiplier = 1f;
    public float selectionWeight = 500;
    public float displayPriority = 1;

    public float minJackpotMarketValue;
    
    //the minimum value something can spawn when the settings.useGlobalConsolationOffset set to true
    public float minConsolationMarketValue = 100f;

    public List<ThingDef> trashOverrideDefs = new();

}