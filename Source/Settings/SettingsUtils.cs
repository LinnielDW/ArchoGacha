namespace ArchoGacha.Settings;

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
    
    public static void DrawLabelledNumericSetting(this Listing_Standard settingsList, ref int settingValue, string settingName, int min, int max)
    {
        var numericSettingRect = settingsList.GetRect(24f);
        var settingValueStringBuffer = settingValue.ToString();

        var leftSide = numericSettingRect.LeftPart(0.8f).Rounded();

        Widgets.Label(leftSide, settingName.Translate());
        TooltipHandler.TipRegion(leftSide, (settingName + "Tooltip").Translate());

        Widgets.TextFieldNumeric(numericSettingRect.RightPart(0.2f).Rounded(), ref settingValue, ref settingValueStringBuffer, min, max);
    }
}