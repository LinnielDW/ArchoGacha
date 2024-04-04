using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

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
        return "ArchoGacha";
    }
}

public class ArchoGachaSettings : ModSettings
{
    public static float bannerDurationDays = 3f;

    public static float minJackpotOffset = 2000f;
    public static float minConsolationOffset = 100f;

    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();

        listingStandard.Begin(inRect);
        listingStandard.DrawLabelledNumericSetting(ref bannerDurationDays,
            "ArchoGacha_bannerDurationDays", 1f,
            999999f);
        listingStandard.DrawLabelledNumericSetting(ref minJackpotOffset,
            "ArchoGacha_minJackpotOffset", 0f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref minConsolationOffset,
            "ArchoGacha_minConsolationOffset", 0f,
            999999f);
        listingStandard.End();
    }
}

public static class SettingsUtils
{
    public static void DrawLabelledNumericSetting<T>(
        this Listing_Standard settingsList, ref T settingValue,
        string settingName, T min, T max) where T : struct
    {
        var settingValueString = settingValue.ToString();
        var numericSettingRect = settingsList.GetRect(24f);

        var leftSide = numericSettingRect.LeftPart(0.8f).Rounded();
        var rightSide = numericSettingRect.RightPart(0.2f).Rounded();

        Widgets.Label(leftSide, settingName.Translate());
        TooltipHandler.TipRegion(leftSide,
            (settingName + "Tooltip").Translate());

        dynamic dynamicMin = min;
        dynamic dynamicMax = max;
        Widgets.TextFieldNumeric<T>(rightSide, ref settingValue,
            ref settingValueString, dynamicMin, dynamicMax);
    }
}