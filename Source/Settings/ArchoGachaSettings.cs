using UnityEngine;
using Verse;

namespace ArchoGacha.Settings;

public class ArchoGachaSettings : ModSettings
{
    public float bannerDurationDays = 3f;

    public float minJackpotOffset = 1500f;
    public bool useGlobalConsolationOffset;
    public float minConsolationOffset = 750f;
    // public static float maxConsolationOffset = 500f;


    public float jackpotChance = 0.005f;

    //TODO: move labels and texts to translations
    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();

        listingStandard.Begin(inRect);
        listingStandard.DrawLabelledNumericSetting(ref bannerDurationDays,
            "ArchoGacha_bannerDurationDays", 1f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref minJackpotOffset,
            "ArchoGacha_minJackpotOffset", 0f, 999999f);
        
        listingStandard.CheckboxLabeled("ArchoGacha_useGlobalConsolationOffset".Translate(), ref useGlobalConsolationOffset, "Use the minConsolationOffset value to generate consolation prizes instead of the usual ");
        listingStandard.DrawLabelledNumericSetting(ref minConsolationOffset,
            "ArchoGacha_minConsolationOffset", 0f, 999999f);
        /*listingStandard.DrawLabelledNumericSetting(ref maxConsolationOffset,
            "maxConsolationOffset", 0f, 999999f);*/
        listingStandard.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref bannerDurationDays, "ArchoGacha_Label_bannerDurationDays", 3f);
        Scribe_Values.Look(ref minJackpotOffset, "ArchoGacha_Label_minJackpotOffset", 1500f);
        Scribe_Values.Look(ref useGlobalConsolationOffset, "ArchoGacha_Label_useGlobalConsolationOffset");
        Scribe_Values.Look(ref minConsolationOffset, "ArchoGacha_Label_minConsolationOffset", 750f);
        base.ExposeData();
    }
}