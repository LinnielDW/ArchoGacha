using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchoGacha.GameComponents;
using HarmonyLib;
using UnityEngine;

namespace ArchoGacha.UI;

[StaticConstructorOnStartup]
public class Dialog_BannerMenu : Window
{
    private GameComponent_GachaTracker comp;
    
    private static Vector2 scrollPosition = Vector2.zero;
    public static PrizeBanner selectedBanner;

    public Dialog_BannerMenu(GameComponent_GachaTracker comp) : base()
    {
        this.comp = comp;
        doCloseButton = false;
        draggable = true;
        doCloseX = true;
        preventCameraMotion = false;
        resizeable = true;
    }

    public override Vector2 InitialSize
    {
        get
        {
            float xSize = 730;
            float ySize = 370;
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
            48 * comp.activeBanners.Count );

        Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);
        listingStandard.Begin(scrollRect);

        foreach (var banner in comp.activeBanners)
        {
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

    public override void PreOpen()
    {
        base.PreOpen();
        selectedBanner = null;
    }

    private void DrawBannerDetailsPanel(Rect inRect,
        Listing_Standard listingStandard, float xOffset)
    {
        listingStandard.NewColumn();
        Rect rghtPanel = new Rect(xOffset + 12f, listingStandard.CurHeight,
            2f * inRect.width / 3f - 12f,inRect.height - listingStandard.CurHeight);
        listingStandard.Begin(rghtPanel);
        Rect rghtInner = new Rect(0f, 0f, rghtPanel.width, rghtPanel.height);
        if (selectedBanner != null)
        {
            Text.Font = GameFont.Medium;
            var bannerTooltip = new StringBuilder().AppendLine($"Probability Breakdown:");
            bannerTooltip.AppendLine($"- The base chance to obtain an ultra rare reward is {settings.jackpotChance.ToStringPercent("0.#")}.");
            bannerTooltip.AppendLine($"     You are guaranteed to obtain an ultra rare reward should your pity reserve exceed {(int)selectedBanner.PityThreshold}.");
            bannerTooltip.AppendLine($"- The base chance to obtain a rare reward is {settings.consolationChance.ToStringPercent("0.#")}.");
            bannerTooltip.AppendLine($"\nBoosted Rate:");
            bannerTooltip.AppendLine($"- The first time an ultra rare reward is obtained, there is a {settings.getFeatured.ToStringPercent("0.#")} chance to obtain the featured reward.");
            bannerTooltip.AppendLine($"     If the ultra rare reward you obtain is not the featured reward, then the next ultra rare reward you obtain is guaranteed to be the featured ultra rare reward.");
            bannerTooltip.AppendLine($"- When a rare reward is obtained, there is a {settings.getConsolationFeatured.ToStringPercent("0.#")} chance to obtain a featured rare reward.");
            bannerTooltip.AppendLine($"- Featured rewards can only be obtained once per banner.");
            bannerTooltip.AppendLine($"- Pity reserves and feature guarantees are shared between banners and carry over between banner refreshes.");
            
            
            var bannerNameRect = listingStandard.Label(selectedBanner.def.LabelCap);

            Text.Font = GameFont.Tiny;
            GUI.color = Color.gray;
            listingStandard.Indent();
            var subtitleRect = listingStandard.Label("ArchoGacha_TimeRemaining".Translate((comp.bannersEndTick - Find.TickManager.TicksGame).ToStringTicksToPeriodVerbose()).ToString());
            listingStandard.Outdent();
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            
            var titleRect = new Rect(0f, bannerNameRect.y, bannerNameRect.width,
                bannerNameRect.height + subtitleRect.height);
            Widgets.DrawHighlightIfMouseover(titleRect);
            TooltipHandler.TipRegion(titleRect,bannerTooltip.ToString().Trim());
            // listingStandard.Label(selectedBanner.def.description);

            #region Draw Prizes

            Text.Font = GameFont.Medium;
            listingStandard.Label("ArchoGacha_JackpotPrizes".Translate(settings.getFeatured.ToStringPercent("0.#")));
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight , listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(8f);
            
            var jackpotPrizeRect = new Rect(0, listingStandard.CurHeight, 42f, 42f);
            DrawJackpot(jackpotPrizeRect, selectedBanner.jackpot, 0, 0);
            listingStandard.Gap(42f);
            listingStandard.Gap(16f);
            
            
            
            Text.Font = GameFont.Medium;
            listingStandard.Label("ArchoGacha_ConsolationPrizes".Translate(settings.getConsolationFeatured.ToStringPercent("0.#")));
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight , listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(8f);
            
            DrawConsolations(listingStandard);
            listingStandard.Gap(42f);
            #endregion

            // listingStandard.Gap(4f);
            // Widgets.DrawLineHorizontal(0, listingStandard.CurHeight , listingStandard.ColumnWidth, Color.gray);
            // listingStandard.Gap(4f);

            #region Pull buttons
            var pullRect = new Rect(0f, rghtInner.yMax - 42f, rghtInner.width / 2f - 4f, 42f);
            var pullTenRect = new Rect(pullRect.width + 4f, pullRect.y, rghtInner.width / 2f - 4f, 42f);
            if (comp.CanPullOnBanner(selectedBanner))
            {
                if (Widgets.ButtonText(pullRect,$"Pull ({(int)selectedBanner.pullPrice} silver)"))
                {
                    if (selectedBanner != null)
                    {
                        comp.PullOnBanner(selectedBanner);
                    }
                }
            }
            else
            {
                GUI.color = new Color(1f, 1f, 1f, 0.1f);
                Widgets.DrawBox(pullRect, 2);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(pullRect, $"Insufficient silver ({(int)selectedBanner.pullPrice} silver)");
                Text.Anchor = TextAnchor.UpperLeft;
            }
                
            if (comp.CanPullTenOnBanner(selectedBanner))
            {
                if (Widgets.ButtonText(pullTenRect,$"Pull x10 ({(int)(selectedBanner.pullPrice * 10)} silver)"))
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
                Widgets.Label(pullTenRect, $"Insufficient silver ({(int)(selectedBanner.pullPrice * 10)} silver)");
                Text.Anchor = TextAnchor.UpperLeft;
            }
            #endregion
        }
        else
        {
            Text.Font = GameFont.Small;
            listingStandard.Label("ArchoGacha_PleaseSelectBanner".Translate().Colorize(new Color(0.9f, 0.9f, 0.3f)));
            
            Text.Font = GameFont.Tiny;
            GUI.color = Color.gray;
            listingStandard.Indent();
            listingStandard.Label("ArchoGacha_TimeRemaining".Translate((comp.bannersEndTick - Find.TickManager.TicksGame).ToStringTicksToPeriodVerbose()).ToString());
            listingStandard.Outdent();
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            
            listingStandard.Gap(4f);
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight , listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(4f);

            var tooltip =
                new StringBuilder().AppendLineTagged($"Probability Breakdown:");
            tooltip.AppendLine($"- The base chance to obtain an ultra rare reward is <color=teal>{settings.jackpotChance.ToStringPercent("0.#")}</color>.");
            tooltip.AppendLine($"     You are guaranteed to obtain an ultra rare reward should your pity reserve exceed the market value of the featured ultra rare reward.");
            tooltip.AppendLine($"- The base chance to obtain a rare reward is <color=teal>{settings.consolationChance.ToStringPercent("0.#")}</color>.");
            tooltip.AppendLine($"\nBoosted Rate:");
            tooltip.AppendLine($"- The first time an ultra rare reward is obtained, there is a <color=teal>{settings.getFeatured.ToStringPercent("0.#")}</color> chance to obtain the featured reward.");
            tooltip.AppendLine($"     If the ultra rare reward you obtain is not the featured reward, then the next ultra rare reward you obtain is guaranteed to be the featured ultra rare reward.");
            tooltip.AppendLine($"- When a rare reward is obtained, there is a <color=teal>{settings.getConsolationFeatured.ToStringPercent("0.#")}</color> chance to obtain a featured rare reward.");
            tooltip.AppendLine($"- Featured rewards can only be obtained once per banner.");
            tooltip.AppendLine($"- Pity reserves and feature guarantees are shared between banners and carry over between banner refreshes.");
            
            listingStandard.Label(tooltip.ToString().Trim().TruncateHeight(rghtInner.width, rghtInner.height - listingStandard.CurHeight));
        }

        listingStandard.End();
    }

    private static void DrawConsolations(Listing_Standard listingStandard)
    {
        if (selectedBanner.consolationPrizes.Count == 0)
        {
            var consPrizeRect = new Rect(0, listingStandard.CurHeight, 42f, 42f);
            DrawConsolation(consPrizeRect, null, 0, 0);
        }
        else
        {
            for (var index = 0; index < selectedBanner.consolationPrizes.Count; index++)
            {
                var consPrizeRect = new Rect(46f * index, listingStandard.CurHeight, 42f, 42f);
                DrawConsolation(consPrizeRect, selectedBanner.consolationPrizes[index], 0, 0);
            }
        }
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
}