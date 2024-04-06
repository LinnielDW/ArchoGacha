﻿using System.Collections.Generic;
using System.Linq;
using ArchoGacha.GameComps;
using ArchoGacha.UI;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ArchoGacha.HarmonyPatches;

//TODO: move this to a comp
[HarmonyPatch(typeof(Building_CommsConsole), "GetFloatMenuOptions")]
public static class Building_CommsConsole_GetFloatMenuOptions_Patch
{
    public static IEnumerable<FloatMenuOption> Postfix(
        IEnumerable<FloatMenuOption> __result, Pawn myPawn,
        Building_CommsConsole __instance)
    {
        var floatOption = GachaMenuFloatOption(__instance);
        if (floatOption != null)
        {
            var opts = __result.ToList();
            opts.Add(floatOption);
            __result = opts;
        }

        return __result;
    }

    public static FloatMenuOption GachaMenuFloatOption(Building_CommsConsole __instance)
    {
        // if (condition)
        {
            var floatMenuOption =
                new FloatMenuOption("GachaMenu".Translate(),
                    delegate
                    {
                        Find.WindowStack.Add(
                            new Dialog_BannerMenu(__instance.Map.GetComponent<MapComponentGachaTracker>()));
                    });
            return floatMenuOption;
        }

        // return null;
    }
}