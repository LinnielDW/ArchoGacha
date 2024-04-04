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

    public static float minJackpotOffset = 1500f;
    public static float minConsolationOffset = 750f;
    public static float maxConsolationOffset = 500f;

    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();

        listingStandard.Begin(inRect);
        listingStandard.DrawLabelledNumericSetting(ref bannerDurationDays,
            "ArchoGacha_bannerDurationDays", 1f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref minJackpotOffset,
            "ArchoGacha_minJackpotOffset", 0f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref minConsolationOffset,
            "ArchoGacha_minConsolationOffset", 0f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref maxConsolationOffset,
            "maxConsolationOffset", 0f, 999999f);
        listingStandard.End();
    }
}

public static class SettingsUtils
{
    public static void DrawLabelledNumericSetting(this Listing_Standard settingsList, ref float settingValue, string settingName, float min, float max)
    {
        var numericSettingRect = settingsList.GetRect(24f);
        var settingValueStringBuffer = settingValue.ToString();

        var leftSide = numericSettingRect.LeftPart(0.8f).Rounded();

        Widgets.Label(leftSide, settingName.Translate());
        TooltipHandler.TipRegion(leftSide, (settingName + "Tooltip").Translate());

        Widgets.TextFieldNumeric(numericSettingRect.RightPart(0.2f).Rounded(), ref settingValue, ref settingValueStringBuffer, min, max);
    }
}