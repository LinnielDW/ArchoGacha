using UnityEngine;
using Verse;

namespace ArchoGacha.Settings;

public class ArchoGachaSettings : ModSettings
{
    public static float bannerDurationDays = 3f;

    public static float minJackpotOffset = 1500f;
    public static float minConsolationOffset = 750f;
    public static float maxConsolationOffset = 500f;


    public static float jackpotChance = 0.005f;

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