using System;
using System.Collections.Generic;
using System.Linq;
using ArchoGacha.MapComponents;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArchoGacha.UI;

[StaticConstructorOnStartup]
public class Dialog_BannerMenu : Window
{
    private MapComponentGachaTracker comp;
    
    private static Vector2 scrollPosition = Vector2.zero;
    public static PrizeBanner selectedBanner;

    public Dialog_BannerMenu(MapComponentGachaTracker comp) : base()
    {
        this.comp = comp;
        doCloseButton = false;
        draggable = true;
        doCloseX = true;
        preventCameraMotion = false;
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
        listingStandard.verticalSpacing = 0;
        listingStandard.Label("ArchoGacha_GachaBanners".Translate());
        listingStandard.verticalSpacing = 2f;

        DrawPityIndicator(listingStandard);

        if (!comp.activeBanners.NullOrEmpty())
        {
            DrawBannerSelectPanel(inRect, listingStandard, out var viewRect);

            DrawBannerDetailsPanel(inRect, listingStandard, viewRect.width);
        }
        listingStandard.End();
    }

    private void DrawPityIndicator(Listing_Standard listingStandard)
    {
        Text.Font = GameFont.Tiny;
        GUI.color = Color.gray;
        listingStandard.Indent();
        listingStandard.Label("ArchoGacha_Pity".Translate(comp.pitySilverReserve));
        listingStandard.Gap(2f);
        listingStandard.Outdent();
        Text.Font = GameFont.Small;
        GUI.color = Color.white;
    }

    private void DrawBannerSelectPanel(Rect inRect,
        Listing_Standard listingStandard, out Rect viewRect)
    {
        viewRect = new Rect(inRect.x, listingStandard.CurHeight, inRect.width  / 3f,
            inRect.height - listingStandard.CurHeight);
        Rect scrollRect = new Rect(0f, 0f, viewRect.width - 16f,
            60f * comp.activeBanners.Count );

        Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);
        listingStandard.Begin(scrollRect);

        foreach (var banner in comp.activeBanners)
        {
            // Rect bannerRect = new Rect(0f, listingStandard.CurHeight, scrollRect.width, 42f);
            var jackpotPrizeRect = new Rect(0f, listingStandard.CurHeight, 42f, 42f);
            DrawJackpot(jackpotPrizeRect, banner.jackpot, 0, 0);

            Rect labelHoverRect = new Rect(jackpotPrizeRect.xMax+2, listingStandard.CurHeight, scrollRect.width - jackpotPrizeRect.xMax+2, 42f);
            Widgets.DrawHighlightIfMouseover(labelHoverRect);
            if (banner == selectedBanner)
            {
                Widgets.DrawHighlightSelected(labelHoverRect);
            }
            if (Widgets.ButtonInvisible(labelHoverRect))
            {
                selectedBanner = banner;
                pulledThings.Clear();
                /*if (selectedBanner != null)
                    {
                        rectCarousel.Clear();
                        if (selectedBanner.jackpot != null)
                        {
                            rectCarousel.Add(DrawJackpotWithThing(selectedBanner.jackpot));
                        }
                        rectCarousel.Add(DrawJackpotWithThing(null));
                        for (var index = 0; index < selectedBanner.consolationPrizes.Count; index++)
                        {
                            rectCarousel.Add(DrawCons(selectedBanner.consolationPrizes[index]));
                        }
                        
                        rectCarousel.Add(DrawCons(null));
                        rectCarousel.Shuffle();
                    }*/
            }
                
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = new Rect(labelHoverRect.position.x + 4, labelHoverRect.position.y, labelHoverRect.size.x - 4, labelHoverRect.size.y);
            Widgets.Label(labelRect, banner.def.LabelCap.Truncate(labelRect.width));
            Text.Anchor = TextAnchor.UpperLeft;
            listingStandard.Gap(48f);
        }

        listingStandard.End();
        Widgets.EndScrollView();
    }

    private void DrawBannerDetailsPanel(Rect inRect,
        Listing_Standard listingStandard, float xOffset)
    {
        listingStandard.NewColumn();
        Rect rghtPanel = new Rect(xOffset, listingStandard.CurHeight,2f * inRect.width / 3f,inRect.height - listingStandard.CurHeight);
        listingStandard.Begin(rghtPanel);
        Rect rghtInner = new Rect(0f, 0f, rghtPanel.width, rghtPanel.height);
        if (selectedBanner != null)
        {
            Text.Font = GameFont.Medium;
            listingStandard.Label(selectedBanner.def.LabelCap);
            Text.Font = GameFont.Small;
            listingStandard.Gap(4f);

            #region Draw Prizes
            var jackpotPrizeRect = new Rect(4f, listingStandard.CurHeight, 42f, 42f);
            DrawJackpot(jackpotPrizeRect, selectedBanner.jackpot, 0, 0);
            var offset = 1;
            if (!comp.lostFiftyFifty && selectedBanner.jackpot != null)
            {
                Rect iconRect = new Rect(jackpotPrizeRect){ x = jackpotPrizeRect.xMax + 4 };
                DrawJackpot(iconRect, null, 0, 0);
                offset += 1;
            }

            for (var index = 0; index < selectedBanner.consolationPrizes.Count; index++)
            {
                var consPrizeRect = new Rect(4f + 46f * (index + offset), listingStandard.CurHeight, 42f, 42f);
                DrawConsolation(consPrizeRect, selectedBanner.consolationPrizes[index], 0, 0);
            }

            var randConsPrizeRect = new Rect(4f + 46f * (selectedBanner.consolationPrizes.Count + offset), listingStandard.CurHeight, 42f, 42f);
            DrawConsolation(randConsPrizeRect, null, 0, 0);
            #endregion
                
            listingStandard.Gap(42f);
            listingStandard.Gap(6f);

            //TODO: put explanation of chances here
            listingStandard.Label(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            );

            #region Pull buttons
            var pullRect = new Rect(0f, rghtInner.yMax - 32f, rghtInner.width / 2f - 4f, 32f);
            var pullTenRect = new Rect(pullRect.width + 4f, pullRect.y, rghtInner.width / 2f - 4f, 32f);
            if (comp.CanPullOnBanner(selectedBanner))
            {
                if (Widgets.ButtonText(pullRect,$"Pull ({selectedBanner.pullPrice} silver)"))
                {
                    if (selectedBanner != null)
                    {
                        comp.PullOnBanner(selectedBanner);
                        /* TODO: Nice to have: carousel
                            isSpinning = true;
                            rectCarousel.Clear();
                            if (selectedBanner.jackpot != null)
                            {
                                rectCarousel.Add(DrawJackpotWithThing(selectedBanner.jackpot));
                            }
                            rectCarousel.Add(DrawJackpotWithThing(null));
                            for (var index = 0; index < selectedBanner.consolationPrizes.Count; index++)
                            {
                                rectCarousel.Add(DrawCons(selectedBanner.consolationPrizes[index]));
                            }

                            rectCarousel.Add(DrawCons(null));
                            rectCarousel.Shuffle();*/
                    }
                }
            }
            else
            {
                GUI.color = new Color(1f, 1f, 1f, 0.1f);
                Widgets.DrawBox(pullRect, 2);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(pullRect, $"Insufficient silver ({selectedBanner.pullPrice} silver)");
                Text.Anchor = TextAnchor.UpperLeft;
            }
                
            if (comp.CanPullTenOnBanner(selectedBanner))
            {
                if (Widgets.ButtonText(pullTenRect,$"Pull x10 ({selectedBanner.pullPrice * 10} silver)"))
                {
                    if (selectedBanner != null)
                    {
                        comp.PullTenOnBanner(selectedBanner);
                    }
                }
            }
            else
            {
                GUI.color = new Color(1f, 1f, 1f, 0.1f);
                Widgets.DrawBox(pullTenRect, 2);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(pullTenRect, $"Insufficient silver ({selectedBanner.pullPrice * 10} silver)");
                Text.Anchor = TextAnchor.UpperLeft;
            }
            #endregion

            /*TODO: nice to have: pull results
                listingStandard.Gap(32f);
                if (pulledThings.Count != 0)
                {
                    for (var index = 0; index < pulledThings.Count; index++)
                    {
                        // var iconRect = new Rect(rghtInner.width/index - (4f + 46F * index)/2, listingStandard.CurHeight, 42f, 42f);
                        
                        var iconRect = new Rect(4f + 46f * index, listingStandard.CurHeight, 42f, 42f);
                        Widgets.DrawBox(iconRect, 2);
                        Widgets.ThingIcon(iconRect.ContractedBy(4f), pulledThings[index]);
                    }
                }*/
            /*TODO: nice to have: carousel
                 if (isSpinning)
                {
                    
                    rightY += 32f;
                    // var modulo = GenTicks.TicksGame % 100;
                    
                    float periodLength = 100f;
                    float relativePosition = GenTicks.TicksGame % periodLength;
                    float maxExtraFactor = 2f;
                    
                    for (var index = 0; index < rectCarousel.Count; index++)
                    {
                        bool looped =
                            (relativePosition ) > (periodLength) - (index) *
                            (periodLength / rectCarousel.Count);
                        //
                        // float height = descending ? (periodLength-relativePosition) * (periodLength / (float)rectCarousel.Count) : relativePosition * (periodLength / (float)rectCarousel.Count);


                        // var scaledFactor = relativePosition/(index * (periodLength / rectCarousel.Count));
                        // var scaledFactor = periodLength % relativePosition;
                        var drawAction = rectCarousel[index];
                        var prizeRect = new Rect(100f + (4f + 28f) * index,
                            rightY, 28f, 28f);
                        drawAction(prizeRect, !looped ? relativePosition -(4f + 28f) : relativePosition - (4f + 28f) *
                            rectCarousel.Count, 0);
                    }
                    rightY += 32f;
                }*/
        }
        else
        {
            Text.Font = GameFont.Medium;
            listingStandard.Label("ArchoGacha_PleaseSelectBanner".Translate());
            Text.Font = GameFont.Small;
                
            listingStandard.Gap(4f);
            listingStandard.Label( "ArchoGacha_PleaseSelectBanner".Translate());
        }

        listingStandard.End();
    }

    private static readonly Texture2D questionMark = ContentFinder<Texture2D>.Get("UI/Overlays/QuestionMark");
    private static readonly Texture2D Gradient = ContentFinder<Texture2D>.Get("UI/Widgets/ArchoGachaGradient");
    private static readonly Color jackpotPrimaryCol = new Color(0.765F, 0.616F, 0.447F);
    private static readonly Color jackpotSecondaryCol = new Color(0.612F, 0.357F, 0.294F);
    private static readonly Color prizePrimaryCol = new Color(0.71f, 0.71f, 0.71f);
    private static readonly Color prizeSecondaryCol = new Color(0.41f, 0.41f, 0.41f);

    #region Draw icon methods
    
    private static void DrawConsolation(Rect prizeRect, Thing prize,
        float x, float y)
    {
        prizeRect.x += x;
        prizeRect.y += y;
        Widgets.DrawBoxSolid(prizeRect, prizeSecondaryCol);
        GUI.color = prizePrimaryCol;
        GUI.DrawTexture(prizeRect, Gradient);
        Widgets.DrawHighlightIfMouseover(prizeRect);
        Rect iconRect = prizeRect.ContractedBy(1f);
        if (prize != null) 
        {
            DrawBannerConsolation(iconRect, prize);
        }
        else
        {
            DrawRandomPrizeRect(prizeRect);
        }
    }

    private static void DrawBannerConsolation(Rect iconRect, Thing prize)
    {
        if (prize.def.DrawMatSingle != null && prize.def.DrawMatSingle.mainTexture != null)
        {
            Widgets.ThingIcon(iconRect, prize);
        }

        if (Widgets.ButtonInvisible(iconRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(prize));
        }
        if (Mouse.IsOver(iconRect))
        {
            string tooltip = prize.LabelNoParenthesisCap.AsTipTitle() +
                             GenLabel.LabelExtras(prize, true, true) + "\n\n" +
                             prize.DescriptionDetailed;
            if (prize.def.useHitPoints)
            {
                tooltip = string.Concat(tooltip, "\n", prize.HitPoints, " / ", prize.MaxHitPoints);
            }

            TooltipHandler.TipRegion(iconRect, tooltip);
        }
    }
    
    private static void DrawJackpot(Rect prizeRect, Thing jackpot, float xOffset,
        float yOffset)
    {
        prizeRect.x += xOffset;
        prizeRect.y += yOffset;
        Widgets.DrawBoxSolid(prizeRect, jackpotSecondaryCol);
        GUI.color = jackpotPrimaryCol;
        GUI.DrawTexture(prizeRect, Gradient);
        Widgets.DrawHighlightIfMouseover(prizeRect);

        Rect iconRect = prizeRect.ContractedBy(1f);
        if (jackpot != null && jackpot.def.DrawMatSingle != null && jackpot.def.DrawMatSingle.mainTexture != null)
        {
            DrawBannerJackpot(iconRect, jackpot);
        }
        else
        {
            DrawRandomPrizeRect(prizeRect);
        }
    }

    private static void DrawRandomPrizeRect(Rect prizeRect)
    {
        Text.Anchor = TextAnchor.MiddleCenter;
        GUI.color = Color.black;
        GUI.DrawTexture(prizeRect.ContractedBy(5f), questionMark);
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        if (Mouse.IsOver(prizeRect))
        {
            string tooltip = "ArchoGacha_ReplacedByRandom".Translate().AsTipTitle() +
                             "\n\n" +
                             "A random item of comparable value to other rewards in the same tier.";

            TooltipHandler.TipRegion(prizeRect, tooltip);
        }
    }

    private static void DrawBannerJackpot(Rect iconRect, Thing jackpot)
    {
        Widgets.ThingIcon(iconRect, jackpot);
        Text.WordWrap = true;
        if (Widgets.ButtonInvisible(iconRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(jackpot));
        }

        if (Mouse.IsOver(iconRect))
        {
            string tooltip = jackpot.LabelNoParenthesisCap.AsTipTitle() +
                             GenLabel.LabelExtras(jackpot, true, true) + "\n\n" +
                             jackpot.DescriptionDetailed;
            if (jackpot.def.useHitPoints)
            {
                tooltip = string.Concat(tooltip, "\n", jackpot.HitPoints, " / ", jackpot.MaxHitPoints);
            }

            TooltipHandler.TipRegion(iconRect, tooltip);
        }
    }

    #endregion
    
    //TODO: nice to haves
    #region NiceToHaves
    
    // private int tickStart;
    // private static bool isSpinning
    public List<Action<Rect,float, float>> rectCarousel = new();
    public static List<Thing> pulledThings = new();


    private static Action<Rect, float, float> DrawCons(Thing thing)
    {
        return (prizeRect, x, y) => DrawConsolation(prizeRect, thing, x, y);
    }

    private static Action<Rect, float, float> DrawJackpotWithThing(Thing thing)
    {
        return (rect, xOffset, yOffset) => DrawJackpot(rect,thing, xOffset, yOffset);
    }

    public static void AddPulled(Thing t)
    {
        if (pulledThings.Count >= 10)
        {
            pulledThings.RemoveAt(0);
        }

        pulledThings.Add(t);
    }

    #endregion
}