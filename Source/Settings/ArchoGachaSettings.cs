using UnityEngine;

namespace ArchoGacha.Settings;

public class ArchoGachaSettings : ModSettings
{
    public float bannerDurationDays = 4f;

    public float minJackpotOffset = 500f;
    public bool useGlobalConsolationOffset;
    public float minConsolationOffset = 250f;
    // public static float maxConsolationOffset = 500f;

    public bool limitBanners = true;
    public int bannerLimit = 4;


    public float jackpotChance = 0.006f;
    public float pullPriceFactor = 1.25f;
    public float consolationChance => jackpotChance *  10f;


    public float getFeatured = 0.5f;
    public float getConsolationFeatured = 0.75f;

    //debug settings:
    public bool debugAlwaysPullable = false;

    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();

        listingStandard.Begin(inRect);
        listingStandard.DrawLabelledNumericSetting(ref bannerDurationDays, "ArchoGacha_bannerDurationDays", 1f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref minJackpotOffset, "ArchoGacha_minJackpotOffset", 0f, 999999f);
        
        // listingStandard.CheckboxLabeled("ArchoGacha_useGlobalConsolationOffset".Translate(), ref useGlobalConsolationOffset, "Use the minConsolationOffset value to generate consolation prizes instead of the usual ");
        // listingStandard.DrawLabelledNumericSetting(ref minConsolationOffset, "ArchoGacha_minConsolationOffset", 0f, 999999f);
        
        listingStandard.CheckboxLabeled("ArchoGacha_limitBanners".Translate(), ref limitBanners);
        listingStandard.DrawLabelledNumericSetting(ref bannerLimit, "ArchoGacha_bannerLimit", 1, 999999);
        listingStandard.DrawLabelledNumericSetting(ref jackpotChance, "ArchoGacha_jackpotChance", 0f, 1f);
        listingStandard.DrawLabelledNumericSetting(ref pullPriceFactor, "ArchoGacha_pullPriceFactor", 0.001f, 999999f);
        listingStandard.DrawLabelledNumericSetting(ref getFeatured, "ArchoGacha_getFeatured", 0f, 1f);
        listingStandard.DrawLabelledNumericSetting(ref getConsolationFeatured, "ArchoGacha_getConsolationFeatured", 0f, 1f);


        Text.Font = GameFont.Medium;
        listingStandard.Label("DevMode");
        Text.Font = GameFont.Small;
        if (Prefs.DevMode)
        {
            listingStandard.CheckboxLabeled("ArchoGacha_debugAlwaysPullable".Translate(), ref debugAlwaysPullable);
        }

        /*listingStandard.DrawLabelledNumericSetting(ref maxConsolationOffset,
            "maxConsolationOffset", 0f, 999999f);*/
        listingStandard.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref bannerDurationDays, "ArchoGacha_Label_bannerDurationDays", 4f);
        Scribe_Values.Look(ref minJackpotOffset, "ArchoGacha_Label_minJackpotOffset", 500f);
        Scribe_Values.Look(ref useGlobalConsolationOffset, "ArchoGacha_Label_useGlobalConsolationOffset");
        Scribe_Values.Look(ref minConsolationOffset, "ArchoGacha_Label_minConsolationOffset", 250f);
        Scribe_Values.Look(ref limitBanners, "ArchoGacha_Label_limitBanners",true);
        Scribe_Values.Look(ref bannerLimit, "ArchoGacha_Label_bannerLimit",4);
        Scribe_Values.Look(ref jackpotChance, "ArchoGacha_Label_jackpotChance",0.006f);
        Scribe_Values.Look(ref pullPriceFactor, "ArchoGacha_Label_pullPriceFactor",1.25f);
        Scribe_Values.Look(ref debugAlwaysPullable, "ArchoGacha_Label_debugAlwaysPullable",false);
        Scribe_Values.Look(ref getFeatured, "ArchoGacha_Label_getFeatured",0.5f);
        Scribe_Values.Look(ref getConsolationFeatured, "ArchoGacha_Label_getConsolationFeatured",0.75f);
        base.ExposeData();
    }
}