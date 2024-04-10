using System.Collections.Generic;

namespace ArchoGacha.GameComponents;

public static class BannerMaker
{
    public static PrizeBanner GenerateBannerFromDef(PrizeBannerDef def)
    {
        var prizeBanner =
            (PrizeBanner)Activator.CreateInstance(def.prizeBannerClass);

        prizeBanner.def = def;
        prizeBanner.jackpot = prizeBanner.GenerateJackpot();
        prizeBanner.pullPrice = prizeBanner.jackpot.MarketValue * 
                                settings.jackpotChance *
                                settings.pullPriceFactor *
                                prizeBanner.jackpot.stackCount;

        AddConsolations(prizeBanner,
            prizeBanner.jackpot.MarketValue * prizeBanner.jackpot.stackCount * settings.consolationChance);
        
        return prizeBanner;
    }
    
    
    private static void AddConsolations(PrizeBanner prizeBanner,
        float valueMaxOverride = 0f)
    {        
        for (int i = 0; i < prizeBanner.def.consolationCountRange.RandomInRange; i++)
        {
            var prize = prizeBanner.GenerateConsolationThing(valueMaxOverride);
            if (prize != null) prizeBanner.consolationPrizes.Add(prize);
        }
    }

    public static PrizeBanner GenerateRandomBanner()
    {
        return GenerateBannerFromDef(DefDatabase<PrizeBannerDef>
            .AllDefsListForReading.RandomElement());
    }
    
    public static IEnumerable<Thing> GenerateConsolations(
        PrizeBanner prizeBanner,
        float valueMaxOverride = 0f)
    {
        for (int i = 0;
             i < prizeBanner.def.consolationCountRange.RandomInRange;
             i++)
        {
            var prize = prizeBanner.GenerateConsolationThing(valueMaxOverride);
            if (prize != null) yield return prize;
        }
    }
}