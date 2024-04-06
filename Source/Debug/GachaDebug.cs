using ArchoGacha.MapComponents;
using LudeonTK;
using Verse;

namespace ArchoGacha.Debug;

public static class GachaDebug
{
    [DebugAction("ArchoGacha", name = "Debug Log Random PrizeBanner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateABanner()
    {
        var banner = Find.CurrentMap.GetComponent<MapComponentGachaTracker>().GenerateRandomBanner();
        Log.Message(banner.ToString());
    }

    [DebugAction("ArchoGacha", name = "Regenerate Active Banners",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateActiveBanners()
    {
        Find.CurrentMap.GetComponent<MapComponentGachaTracker>().GenerateActiveBanners();
    }
    
    [DebugAction("ArchoGacha", name = "Pull jackpot on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPullJackpot()
    {
        var comp = Find.CurrentMap.GetComponent<MapComponentGachaTracker>();
        var banner = comp.activeBanners.RandomElement();
        var prize = comp.SelectJackpot(banner);
        comp.DeliverPrize(prize);
    }
    
    [DebugAction("ArchoGacha", name = "Simulate pull on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPull()
    {
        var comp = Find.CurrentMap.GetComponent<MapComponentGachaTracker>();
        var prize = comp.DecidePrize(comp.activeBanners.RandomElement());
        comp.DeliverPrize(prize);
    }
    
    [DebugAction("ArchoGacha", name = "(x10) Simulate pull on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPullTen()
    {
        var comp = Find.CurrentMap.GetComponent<MapComponentGachaTracker>();
        var selectBanner = comp.activeBanners.RandomElement();
        for (int i = 0; i < 10; i++)
        {
            var prize = comp.DecidePrize(selectBanner);
            Log.Message(prize.ToString());
            comp.DeliverPrize(prize);
        }
    }
}