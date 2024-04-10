using System.Collections.Generic;
using System.Linq;
using ArchoGacha.GameComponents;
using HarmonyLib;

namespace ArchoGacha.HarmonyPatches;

[HarmonyPatch(typeof(Building_CommsConsole), nameof(Building_CommsConsole.GetCommTargets))]
public static class Building_CommsConsole_GetCommTargets_Patch
{
    public static IEnumerable<ICommunicable> Postfix(IEnumerable<ICommunicable> targets) => targets.Append(Current.Game.GetComponent<GameComponent_GachaTracker>());
}