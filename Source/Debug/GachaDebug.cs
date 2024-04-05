using ArchoGacha.GameComps;
using LudeonTK;
using Verse;

namespace ArchoGacha.Debug;

public static class GachaDebug
{
    [DebugAction("ArchoGacha", name = "Debug Output Random PrizeBanner",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateABanner()
    {
        var banner = Find.CurrentMap.GetComponent<MapComponentGachaTracker>().GenerateRandomBanner();
        Log.Message(banner.ToString());
    }

    [DebugAction("ArchoGacha", name = "RegenerateActiveBanners",
        allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateActiveBanners()
    {
        Find.CurrentMap.GetComponent<MapComponentGachaTracker>().GenerateActiveBanners();
        // Log.Message(banner.ToString());
    }
}