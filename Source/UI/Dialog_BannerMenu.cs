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
        var titleRect = listingStandard.Label("ArchoGacha_GachaBanners".Translate());
        listingStandard.verticalSpacing = 2f;

        Text.Font = GameFont.Tiny;
        GUI.color = Color.gray;
        listingStandard.Indent();
        listingStandard.Label("ArchoGacha_Pity".Translate(comp.pitySilverReserve));
        // listingStandard.Label("ArchoGacha_PityCount".Translate(comp.pityCount));
        listingStandard.Gap(2f);
        listingStandard.Outdent();
        Text.Font = GameFont.Small;
        GUI.color = Color.white;

        if (!comp.activeBanners.NullOrEmpty())
        {
            Rect viewRect = new Rect(inRect.x, listingStandard.CurHeight, inRect.width  / 3f,
                inRect.height - listingStandard.CurHeight);
            Rect scrollRect = new Rect(0f, 0f, viewRect.width - 16f,
                60f * comp.activeBanners.Count );


            Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);
            // listingStandard.ColumnWidth = viewRect.width - 16f;
            listingStandard.Begin(scrollRect);

            // var y = 0f;

            foreach (var banner in comp.activeBanners)
            {
                Rect bannerRect = new Rect(0f, listingStandard.CurHeight, scrollRect.width, 42f);
                
                
                var jackpotPrizeRect = new Rect(0f, listingStandard.CurHeight, 42f, 42f);
                DrawJackpot(jackpotPrizeRect, banner.jackpot, 0, 0);
                
                Rect labelHoverRect = new Rect(jackpotPrizeRect.xMax+2, bannerRect.y, bannerRect.width - jackpotPrizeRect.xMax+2, 42f);
                
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect labelRect = new Rect(labelHoverRect.position.x + 4, labelHoverRect.position.y, labelHoverRect.size.x - 4, labelHoverRect.size.y);
                Widgets.Label(labelRect, banner.def.LabelCap.Truncate(labelRect.width));
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.DrawHighlightIfMouseover(labelHoverRect);
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
                listingStandard.Gap(48f);
                /*
                 
                DrawJackpot(jackpotPrizeRect, banner.jackpot, 0, 0);
                for (var index = 0; index < banner.consolationPrizes.Count; index++)
                {
                    
                    var consPrizeRect = new Rect(4f + (4f + 28f) * (index + 1), y, 28f, 28f);
                    DrawConsolation(consPrizeRect, banner.consolationPrizes[index], 0, 0, index);
                }

                y += 32f;
                */
            }

            listingStandard.End();
            Widgets.EndScrollView();


            listingStandard.NewColumn();
            Rect rghtPanel = new Rect(viewRect.width, listingStandard.CurHeight,2f * inRect.width / 3f,inRect.height - listingStandard.CurHeight);
            listingStandard.Begin(rghtPanel);
            Rect rghtInner = new Rect(0f, 0f, rghtPanel.width, rghtPanel.height);
            if (selectedBanner != null)
            {
                // var rightY = 0f;
                Text.Font = GameFont.Medium;
                // Widgets.Label(rghtInner, selectedBanner.def.LabelCap);
                listingStandard.Label(selectedBanner.def.LabelCap);
                Text.Font = GameFont.Small;
                // rightY += Text.CalcHeight(selectedBanner.def.LabelCap, rghtInner.width);
                
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
                
                listingStandard.Gap(42f);
                listingStandard.Gap(4f);
                // listingStandard.CurHeight += 42;
                // listingStandard.CurHeight += 4;

                
                var pullRect = new Rect(0f, listingStandard.CurHeight, rghtInner.width / 2f - 4f, 32f);
                var pullTenRect = new Rect(pullRect.width + 4f, listingStandard.CurHeight, rghtInner.width / 2f - 4f, 32f);
                
                // rightY += 32f;
                listingStandard.Gap(32f);
                if (comp.CanPullOnBanner(selectedBanner))
                {
                    if (Widgets.ButtonText(pullRect,$"Pull ({selectedBanner.pullPrice} silver)"))
                    {
                        if (selectedBanner != null)
                        {
                            comp.PullOnBanner(selectedBanner);
                            /*rectCarousel.Clear();
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

                        // isSpinning = true;
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

                //TODO: nice to have: finish this
                /*if (isSpinning)
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
                Widgets.Label(rghtInner, "ArchoGacha_PleaseSelectBanner".Translate());
            }

            listingStandard.End();
        }
        listingStandard.End();
    }

    // private int tickStart;

    private static Action<Rect, float, float> DrawCons(Thing thing)
    {
        return (prizeRect, x, y) => DrawConsolation(prizeRect, thing, x, y);
    }
    private static void DrawConsolation(Rect prizeRect, Thing prize,
        float x, float y)
    {
        prizeRect.x += x;
        prizeRect.y += y;
        Widgets.DrawBoxSolid(prizeRect, prizeSecondaryCol);
        Rect iconRect = prizeRect.ContractedBy(1f);
        GUI.color = prizePrimaryCol;
        GUI.DrawTexture(iconRect, Gradient);
        Widgets.DrawHighlightIfMouseover(iconRect);
        if (prize != null) 
        {
            DrawBannerConsolation(iconRect, prize);
        }
        else
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = Color.black;
            GUI.DrawTexture(prizeRect.ContractedBy(5f), questionMark);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            if (Mouse.IsOver(iconRect))
            {
                string tooltip = "ArchoGacha_ReplacedByRandom".Translate().AsTipTitle() +
                                 "\n\n" +
                                 "A random item of comparable value to other rewards in the same tier.";

                TooltipHandler.TipRegion(iconRect, tooltip);
            }
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

    private static Action<Rect, float, float> DrawJackpotWithThing(Thing thing)
    {
        return (rect, xOffset, yOffset) => DrawJackpot(rect,thing, xOffset, yOffset);
    }

    private static void DrawJackpot(Rect prizeRect, Thing jackpot, float xOffset,
        float yOffset)
    {
        prizeRect.x += xOffset;
        prizeRect.y += yOffset;
        Widgets.DrawBoxSolid(prizeRect, jackpotSecondaryCol);
        Rect iconRect = prizeRect.ContractedBy(1f);
        GUI.color = jackpotPrimaryCol;
        GUI.DrawTexture(iconRect, Gradient);
        Widgets.DrawHighlightIfMouseover(iconRect);
                    
        if (jackpot != null && jackpot.def.DrawMatSingle != null && jackpot.def.DrawMatSingle.mainTexture != null)
        {
            DrawBannerJackpot(jackpot, iconRect);
        }
        else
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = Color.black;
            GUI.DrawTexture(prizeRect.ContractedBy(5f), questionMark);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            if (Mouse.IsOver(iconRect))
            {
                string tooltip = "ArchoGacha_ReplacedByRandom".Translate().AsTipTitle() +
                                 "\n\n" +
                                 "This reward will be a random item of comparable value to other rewards in the same tier.";

                TooltipHandler.TipRegion(iconRect, tooltip);
            }
        }
    }

    private static void DrawBannerJackpot(Thing jackpot, Rect iconRect)
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

    public static List<Thing> pulledThings = new();

    public static void AddPulled(Thing t)
    {
        if (pulledThings.Count >= 10)
        {
            pulledThings.RemoveAt(0);
        }

        pulledThings.Add(t);
    }

    public static PrizeBanner selectedBanner;
    // private static bool isSpinning;


    public List<Action<Rect,float, float>> rectCarousel = new();

    private static readonly Texture2D questionMark = ContentFinder<Texture2D>.Get("UI/Overlays/QuestionMark");
    private static readonly Texture2D Gradient = ContentFinder<Texture2D>.Get("UI/Widgets/ArchoGachaGradient");
    private static readonly Color jackpotPrimaryCol = new Color(0.765F, 0.616F, 0.447F);
    private static readonly Color jackpotSecondaryCol = new Color(0.612F, 0.357F, 0.294F);
    private static readonly Color prizePrimaryCol = new Color(0.71f, 0.71f, 0.71f);
    private static readonly Color prizeSecondaryCol = new Color(0.41f, 0.41f, 0.41f);
}