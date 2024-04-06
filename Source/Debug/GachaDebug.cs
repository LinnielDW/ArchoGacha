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
}