using ArchoGacha.GameComps;
using LudeonTK;
using Verse;

namespace ArchoGacha.PrizeWorkers;

public static class GachaDebug
{
    [DebugAction("ArchoGacha", name = "Generate Random Banner", allowedGameStates = AllowedGameStates.Playing,
        displayPriority = 9999)]
    private static void GenerateABanner()
    {
        var banner = GameComponent_GachaTracker.Instance.GenerateRandomBanner();
        Log.Message(banner.ToString());
    }
}