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
        var banner = Current.Game.GetComponent<GameComponent_GachaTracker>().GenerateRandomBanner();
        Log.Message(banner.ToString());
    }
}