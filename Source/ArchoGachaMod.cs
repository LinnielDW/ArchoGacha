using System.Reflection;
using ArchoGacha.Settings;
using HarmonyLib;
using UnityEngine;

namespace ArchoGacha;

public class ArchoGachaMod : Mod
{
    public static ArchoGachaSettings settings;

    public ArchoGachaMod(ModContentPack content) : base(content)
    {
        settings = GetSettings<ArchoGachaSettings>();
        var harmony = new Harmony("com.arquebus.rimworld.mod.archogacha");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoSettingsWindowContents(inRect);
    }
    public override string SettingsCategory()
    {
        return "ArchoGachaCategory".Translate();
    }
}