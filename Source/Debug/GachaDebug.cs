using ArchoGacha.MapComponents;
using LudeonTK;

namespace ArchoGacha.Debug;

public static class GachaDebug
{
    [DebugAction("ArchoGacha", name = "Debug Log Random PrizeBanner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateABanner()
    {
        var banner = Current.Game.GetComponent<MapComponentGachaTracker>().GenerateRandomBanner();
        Log.Message(banner.ToString());
    }

    [DebugAction("ArchoGacha", name = "Regenerate Active Banners",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateActiveBanners()
    {
        Current.Game.GetComponent<MapComponentGachaTracker>().GenerateActiveBanners();
    }
    
    [DebugAction("ArchoGacha", name = "Pull jackpot on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPullJackpot()
    {
        var comp = Current.Game.GetComponent<MapComponentGachaTracker>();
        var banner = comp.activeBanners.RandomElement();
        var prize = comp.SelectJackpot(banner);
        comp.DeliverPrize(prize);
    }    
    [DebugAction("ArchoGacha", name = "Pull consolation on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPullConsolation()
    {
        var comp = Current.Game.GetComponent<MapComponentGachaTracker>();
        var banner = comp.activeBanners.RandomElement();
        var prize = comp.SelectConsolation(banner);
        comp.DeliverPrize(prize);
    }
    
    [DebugAction("ArchoGacha", name = "Simulate pull on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPull()
    {
        var comp = Current.Game.GetComponent<MapComponentGachaTracker>();
        var prize = comp.DecidePrize(comp.activeBanners.RandomElement());
        comp.DeliverPrize(prize);
    }
    
    [DebugAction("ArchoGacha", name = "(x10) Simulate pull on random active banner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void DoPullTen()
    {
        var comp = Current.Game.GetComponent<MapComponentGachaTracker>();
        var selectBanner = comp.activeBanners.RandomElement();
        for (int i = 0; i < 10; i++)
        {
            var prize = comp.DecidePrize(selectBanner);
            Log.Message(prize.ToString());
            comp.DeliverPrize(prize);
        }
    }
}