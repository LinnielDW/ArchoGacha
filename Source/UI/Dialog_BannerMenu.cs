using ArchoGacha.GameComps;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArchoGacha.UI;

[StaticConstructorOnStartup]
public class Dialog_BannerMenu : Window
{
    private GameComponent_GachaTracker comp;
    private static Vector2 scrollPosition = Vector2.zero;

    public Dialog_BannerMenu(GameComponent_GachaTracker comp) : base()
    {
        this.comp = comp;
        this.doCloseButton = true;
    }

    public override Vector2 InitialSize
    {
        get
        {
            float xSize = 800;
            float ySize = 380;
            return new Vector2(xSize, ySize);
        }
    }


    public override void DoWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);

        Text.Font = GameFont.Medium;
        listingStandard.Label("GachaBanners".Translate());

        if (!comp.activeBanners.NullOrEmpty())
        {
            Rect viewRect = new Rect(inRect.x, listingStandard.CurHeight, inRect.width  / 3f,
                inRect.height - listingStandard.CurHeight);
            Rect scrollRect = new Rect(0f, 0f, viewRect.width - 16f,
                60f * comp.activeBanners.Count );


            Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect, true);
            listingStandard.ColumnWidth = (scrollRect.width - 17f);
            listingStandard.Begin(scrollRect);

            var y = 0f;

            foreach (var banner in comp.activeBanners)
            {
                //TODO: finish
                Rect rect = new Rect(0f, y, scrollRect.width, 56f);
                Widgets.DrawHighlightIfMouseover(rect);
                Widgets.Label(rect, banner.def.LabelCap);
                y += 28f;

                // string text = banner.jackpot.LabelCap;
                // Rect rect5 = new Rect(36f, y, rect.width - 36f, rect.height);
                // Widgets.Label(rect5, text.Truncate(rect5.width, null));
                
                // y += 28f;
                
                if (banner.jackpot.def.DrawMatSingle != null && banner.jackpot.def.DrawMatSingle.mainTexture != null)
                {
                    var iconRect = new Rect(4f, y, 28f, 28f);
                    
                    // GUI.color = Color.white;
                    // Widgets.DrawBox(iconRect, 1, null);
                    Rect rect2 = iconRect.ContractedBy(1f);
                    
                    // GUI.DrawTexture(rect2, ColonistBarColonistDrawer.MoodBGTex);
                    Widgets.DrawBoxSolid(rect2, jackpotSecondaryCol);
                    GUI.color = jackpotPrimaryCol;
                    GUI.DrawTexture(rect2, Gradient);
                    // Widgets.DrawBoxSolidWithOutline(iconRect, jackpotPrimaryCol,jackpotSecondaryCol);
                    Widgets.DrawHighlightIfMouseover(rect2);
                    
                    Text.WordWrap = true;
                    if (Mouse.IsOver(rect2))
                    {
                        string text2 = banner.jackpot.LabelNoParenthesisCap.AsTipTitle() +
                                       GenLabel.LabelExtras(banner.jackpot, true, true) + "\n\n" +
                                       banner.jackpot.DescriptionDetailed;
                        if (banner.jackpot.def.useHitPoints)
                        {
                            text2 = string.Concat(new object[]
                                { text2, "\n", banner.jackpot.HitPoints, " / ", banner.jackpot.MaxHitPoints });
                        }

                        TooltipHandler.TipRegion(rect2, text2);
                    }

                    /*if (Mouse.IsOver(iconRect))
                    {
                        GUI.color = ITab_Pawn_Gear.HighlightColor;
                        GUI.DrawTexture(iconRect, TexUI.HighlightTex);
                    }*/
                    Widgets.ThingIcon(rect2, banner.jackpot, 1f, null, false);
                    if (Widgets.ButtonInvisible(rect2, true))
                    {
                        Find.WindowStack.Add(new Dialog_InfoCard(banner.jackpot));
                    }
                }

                for (var index = 0; index < banner.prizes.Count; index++)
                {
                    var prize = banner.prizes[index];
                    if (prize.def.DrawMatSingle != null &&
                        prize.def.DrawMatSingle.mainTexture != null)
                    {
                        var iconRect = new Rect(4f + (4f + 28f) * (index + 1), y, 28f, 28f);
                        // GUI.color = Color.white;
                        // Widgets.DrawBox(iconRect, 1, null);
                        Rect rect2 = iconRect.ContractedBy(1f);
                        Widgets.DrawBoxSolid(rect2, prizeSecondaryCol);
                        GUI.color = prizePrimaryCol;
                        GUI.DrawTexture(rect2, Gradient);
                        // Widgets.DrawBoxSolidWithOutline(iconRect, jackpotSecondaryCol,jackpotPrimaryCol);
                        Widgets.DrawHighlightIfMouseover(rect2);
                        if (Mouse.IsOver(rect2))
                        {
                            string text2 = prize.LabelNoParenthesisCap.AsTipTitle() +
                                           GenLabel.LabelExtras(prize, true, true) + "\n\n" +
                                           prize.DescriptionDetailed;
                            if (prize.def.useHitPoints)
                            {
                                text2 = string.Concat(text2, "\n", prize.HitPoints, " / ", prize.MaxHitPoints);
                            }

                            TooltipHandler.TipRegion(rect2, text2);
                        }
                        
                        Widgets.ThingIcon(rect2, prize, 1f, null, false);
                        if (Widgets.ButtonInvisible(rect2, true))
                        {
                            Find.WindowStack.Add(new Dialog_InfoCard(prize));
                        }
                    }
                }

                // y += 28f;
                y += 32f;
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        listingStandard.End();
    }

    private static readonly Texture2D Gradient = ContentFinder<Texture2D>.Get("UI/Widgets/ArchoGachaGradient", true);
    private static readonly Color jackpotPrimaryCol = new Color(0.765F, 0.616F, 0.447F);
    private static readonly Color jackpotSecondaryCol = new Color(0.612F, 0.357F, 0.294F);
    private static readonly Color prizePrimaryCol = new Color(0.71f, 0.71f, 0.71f);
    private static readonly Color prizeSecondaryCol = new Color(0.41f, 0.41f, 0.41f);
}