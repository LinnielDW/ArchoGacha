using System.Text;
using ArchoGacha.GameComponents;
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
        listingStandard.Label(
            "ArchoGacha_Pity".Translate(comp.pitySilverReserve));
        listingStandard.Outdent();
        Text.Font = GameFont.Small;
        GUI.color = Color.white;
    }

    private void DrawBannerSelectPanel(Rect inRect,
        Listing_Standard listingStandard, out Rect viewRect)
    {
        viewRect = new Rect(inRect.x, listingStandard.CurHeight,
            inRect.width / 3f,
            inRect.height - listingStandard.CurHeight);
        Rect scrollRect = new Rect(0f, 0f, viewRect.width - 16f,
            48 * comp.activeBanners.Count);

        Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);
        listingStandard.Begin(scrollRect);

        foreach (var banner in comp.activeBanners)
        {
            var jackpotPrizeRect =
                new Rect(0f, listingStandard.CurHeight, 42f, 42f);
            DrawJackpot(jackpotPrizeRect, banner.jackpot, 0, 0, banner.def.drawSizeFactor);

            Rect labelHoverRect = new Rect(jackpotPrizeRect.xMax + 2,
                listingStandard.CurHeight,
                scrollRect.width - jackpotPrizeRect.xMax + 2, 42f);
            Widgets.DrawHighlightIfMouseover(labelHoverRect);
            if (banner == selectedBanner)
            {
                Widgets.DrawHighlightSelected(labelHoverRect);
            }

            if (Widgets.ButtonInvisible(labelHoverRect))
            {
                selectedBanner = selectedBanner != banner ? banner : null;
            }

            Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = new Rect(labelHoverRect.position.x + 4,
                labelHoverRect.position.y, labelHoverRect.size.x - 4,
                labelHoverRect.size.y);
            Widgets.Label(labelRect,
                banner.def.LabelCap.Truncate(labelRect.width));
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
            2f * inRect.width / 3f - 12f,
            inRect.height - listingStandard.CurHeight);
        listingStandard.Begin(rghtPanel);
        Rect rghtInner = new Rect(0f, 0f, rghtPanel.width, rghtPanel.height);
        if (selectedBanner != null)
        {
            Text.Font = GameFont.Medium;
            var pityThresholdString = selectedBanner.PityThreshold.ToString();
            var bannerTooltip = new StringBuilder();
            bannerTooltip.AppendLine("ArchoGacha_Instructions_ProbBreakdown"
                .Translate());
            bannerTooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_ProbJackpot".Translate().Resolve(),
                settings.jackpotChance.ToStringPercent("0.#").Colorize(teal)));
            bannerTooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_ProbPity".Translate().Resolve(),
                pityThresholdString.Colorize(teal)));
            bannerTooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_ProbConsolation".Translate().Resolve(),
                settings.consolationChance.ToStringPercent("0.#")
                    .Colorize(teal)));
            bannerTooltip.AppendLine(
                "ArchoGacha_Instructions_Rates".Translate());
            bannerTooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_RatesJackpot".Translate().Resolve(),
                settings.getFeatured.ToStringPercent("0.#").Colorize(teal)));
            bannerTooltip.AppendLine("ArchoGacha_Instructions_RatesJackpotLoss"
                .Translate());
            bannerTooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_RatesConsolation".Translate()
                    .Resolve(),
                settings.getConsolationFeatured.ToStringPercent("0.#")
                    .Colorize(teal)));
            bannerTooltip.AppendLine("ArchoGacha_Instructions_FeatureSingleton"
                .Translate());
            bannerTooltip.AppendLine("ArchoGacha_Instructions_PityTransfer"
                .Translate());


            var bannerNameRect =
                listingStandard.Label(selectedBanner.def.LabelCap);

            Text.Font = GameFont.Tiny;
            GUI.color = Color.gray;
            listingStandard.Indent();
            var subtitleRect = listingStandard.Label("ArchoGacha_TimeRemaining"
                .Translate((comp.bannersEndTick - Find.TickManager.TicksGame)
                    .ToStringTicksToPeriodVerbose()).ToString());
            listingStandard.Outdent();
            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            var titleRect = new Rect(0f, bannerNameRect.y, bannerNameRect.width,
                bannerNameRect.height + subtitleRect.height);
            Widgets.DrawHighlightIfMouseover(titleRect);
            TooltipHandler.TipRegion(titleRect,
                bannerTooltip.ToString().Trim());
            // listingStandard.Label(selectedBanner.def.description);

            #region Draw Prizes

            Text.Font = GameFont.Medium;
            listingStandard.Label(
                "ArchoGacha_JackpotPrizes".Translate(
                    settings.getFeatured.ToStringPercent("0.#")));
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight,
                listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(8f);

            var jackpotPrizeRect =
                new Rect(0, listingStandard.CurHeight, 42f, 42f);
            DrawJackpot(jackpotPrizeRect, selectedBanner.jackpot, 0, 0, selectedBanner.def.drawSizeFactor);
            listingStandard.Gap(42f);
            listingStandard.Gap(16f);


            Text.Font = GameFont.Medium;
            listingStandard.Label("ArchoGacha_ConsolationPrizes".Translate(
                settings.getConsolationFeatured.ToStringPercent("0.#")));
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight,
                listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(8f);

            DrawConsolations(listingStandard);
            listingStandard.Gap(42f);

            #endregion

            // listingStandard.Gap(4f);
            // Widgets.DrawLineHorizontal(0, listingStandard.CurHeight , listingStandard.ColumnWidth, Color.gray);
            // listingStandard.Gap(4f);

            #region Pull buttons

            var pullRect = new Rect(0f, rghtInner.yMax - 42f,
                rghtInner.width / 2f - 4f, 42f);
            var pullTenRect = new Rect(pullRect.width + 4f, pullRect.y,
                rghtInner.width / 2f - 4f, 42f);
            if (comp.CanPullOnBanner(selectedBanner))
            {
                if (Widgets.ButtonText(pullRect,
                        $"Pull ({selectedBanner.pullPrice} silver)"))
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
                Widgets.Label(pullRect,
                    $"Insufficient silver ({selectedBanner.pullPrice} silver)");
                Text.Anchor = TextAnchor.UpperLeft;
            }

            if (comp.CanPullTenOnBanner(selectedBanner))
            {
                if (Widgets.ButtonText(pullTenRect,
                        $"Pull x10 ({selectedBanner.pullPrice * 10} silver)"))
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
                Widgets.Label(pullTenRect,
                    $"Insufficient silver ({selectedBanner.pullPrice * 10} silver)");
                Text.Anchor = TextAnchor.UpperLeft;
            }

            #endregion
        }
        else
        {
            Text.Font = GameFont.Small;
            listingStandard.Label("ArchoGacha_PleaseSelectBanner".Translate()
                .Colorize(yellow));

            Text.Font = GameFont.Tiny;
            GUI.color = Color.gray;
            listingStandard.Indent();
            listingStandard.Label("ArchoGacha_TimeRemaining"
                .Translate((comp.bannersEndTick - Find.TickManager.TicksGame)
                    .ToStringTicksToPeriodVerbose()).ToString());
            listingStandard.Outdent();
            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            listingStandard.Gap(4f);
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight,
                listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(4f);

            var glossaryTooltip = new StringBuilder();
            glossaryTooltip.AppendLine("Archogacha_Explaination_Gacha".Translate());
            glossaryTooltip.AppendLine("Archogacha_Explaination_Banner".Translate());
            glossaryTooltip.AppendLine("Archogacha_Explaination_Pull".Translate());
            
            listingStandard.Label(glossaryTooltip.ToString().Trim()
                .TruncateHeight(rghtInner.width,
                    rghtInner.height - listingStandard.CurHeight));
            
            
            Widgets.DrawLineHorizontal(0, listingStandard.CurHeight,
                listingStandard.ColumnWidth, Color.gray);
            listingStandard.Gap(4f);


            var tooltip = new StringBuilder();
            
            tooltip.AppendLine(
                "ArchoGacha_Instructions_ProbBreakdown".Translate());
            tooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_ProbJackpot".Translate().Resolve(),
                settings.jackpotChance.ToStringPercent("0.#").Colorize(teal)));
            tooltip.AppendLine("ArchoGacha_Instructions_ProbPityUncalculated"
                .Translate());
            tooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_ProbConsolation".Translate().Resolve(),
                settings.consolationChance.ToStringPercent("0.#")
                    .Colorize(teal)));
            tooltip.AppendLine("ArchoGacha_Instructions_Rates".Translate());
            tooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_RatesJackpot".Translate().Resolve(),
                settings.getFeatured.ToStringPercent("0.#").Colorize(teal)));
            tooltip.AppendLine("ArchoGacha_Instructions_RatesJackpotLoss"
                .Translate());
            tooltip.AppendLine(string.Format(
                "ArchoGacha_Instructions_RatesConsolation".Translate()
                    .Resolve(),
                settings.getConsolationFeatured.ToStringPercent("0.#")
                    .Colorize(teal)));
            tooltip.AppendLine("ArchoGacha_Instructions_FeatureSingleton"
                .Translate());
            tooltip.AppendLine(
                "ArchoGacha_Instructions_PityTransfer".Translate());

            listingStandard.Label(tooltip.ToString().Trim()
                .TruncateHeight(rghtInner.width,
                    rghtInner.height - listingStandard.CurHeight));
        }

        listingStandard.End();
    }

    private static void DrawConsolations(Listing_Standard listingStandard)
    {
        if (selectedBanner.consolationPrizes.Count == 0)
        {
            var consPrizeRect =
                new Rect(0, listingStandard.CurHeight, 42f, 42f);
            DrawConsolation(consPrizeRect, null, 0, 0);
        }
        else
        {
            for (var index = 0;
                 index < selectedBanner.consolationPrizes.Count;
                 index++)
            {
                var consPrizeRect = new Rect(46f * index,
                    listingStandard.CurHeight, 42f, 42f);
                DrawConsolation(consPrizeRect,
                    selectedBanner.consolationPrizes[index], 0, 0, selectedBanner.def.drawSizeFactor);
            }
        }
    }

    private static readonly Texture2D questionMark =
        ContentFinder<Texture2D>.Get("UI/Overlays/QuestionMark");

    private static readonly Texture2D Gradient =
        ContentFinder<Texture2D>.Get("UI/Widgets/ArchoGachaGradient");

    private static readonly Color jackpotPrimaryCol =
        new Color(0.765F, 0.616F, 0.447F);

    private static readonly Color jackpotSecondaryCol =
        new Color(0.612F, 0.357F, 0.294F);

    private static readonly Color prizePrimaryCol =
        new Color(0.71f, 0.71f, 0.71f);

    private static readonly Color prizeSecondaryCol =
        new Color(0.41f, 0.41f, 0.41f);

    private static readonly Color teal = new Color(0.008F, 0.58F, 0.53F);
    private static readonly Color yellow = new Color(0.9f, 0.9f, 0.3f);

    #region Draw icon methods
    
    private static void DrawConsolation(Rect prizeRect, Thing prize,
        float x, float y, float drawSizeFactor = 1)
    {
        prizeRect.x += x;
        prizeRect.y += y;
        Widgets.DrawBoxSolid(prizeRect, prizeSecondaryCol);
        GUI.color = prizePrimaryCol;
        GUI.DrawTexture(prizeRect, Gradient);
        Widgets.DrawHighlightIfMouseover(prizeRect);
        Rect iconRect = prizeRect.ContractedBy(1f);
        if (prize != null && prize.def.DrawMatSingle != null &&
            prize.def.DrawMatSingle.mainTexture != null)
        {
            DrawBannerConsolation(iconRect, prize, drawSizeFactor);
        }
        else
        {
            DrawRandomPrizeRect(prizeRect);
        }
    }

    private static void DrawBannerConsolation(Rect iconRect, Thing prize, float drawSizeFactor = 1)
    {
        var scaledIconRect = iconRect.ScaledBy(drawSizeFactor);
        Widgets.ThingIcon(scaledIconRect, prize);
        if (prize.stackCount > 1)
        {
            Text.Anchor = TextAnchor.LowerRight;
            Text.Font = GameFont.Tiny;
            Widgets.Label(iconRect, prize.stackCount.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        Text.WordWrap = true;
        if (Widgets.ButtonInvisible(iconRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(prize));
        }

        if (Mouse.IsOver(iconRect))
        {
            string tooltip = prize.Label.AsTipTitle() + "\n\n" +
                             prize.DescriptionDetailed;
            if (prize.def.useHitPoints)
            {
                tooltip = string.Concat(tooltip, "\n", prize.HitPoints, " / ",
                    prize.MaxHitPoints);
            }

            TooltipHandler.TipRegion(iconRect, tooltip);
        }
    }

    private static void DrawJackpot(Rect prizeRect, Thing jackpot,
        float xOffset,
        float yOffset, float drawSizeFactor = 1)
    {
        // var jackpot = banner.jackpot;
        prizeRect.x += xOffset;
        prizeRect.y += yOffset;
        Widgets.DrawBoxSolid(prizeRect, jackpotSecondaryCol);
        GUI.color = jackpotPrimaryCol;
        GUI.DrawTexture(prizeRect, Gradient);
        Widgets.DrawHighlightIfMouseover(prizeRect);

        Rect iconRect = prizeRect.ContractedBy(1f);
        if (jackpot != null && jackpot.def.DrawMatSingle != null &&
            jackpot.def.DrawMatSingle.mainTexture != null)
        {
            DrawBannerJackpot(iconRect, jackpot, drawSizeFactor);
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
            string tooltip =
                "ArchoGacha_ReplacedByRandom".Translate().Resolve()
                    .AsTipTitle() +
                "\n\n" +
                "ArchoGacha_ReplacedByRandomDesc".Translate();

            TooltipHandler.TipRegion(prizeRect, tooltip);
        }
    }

    private static void DrawBannerJackpot(Rect iconRect,  Thing jackpot, float drawSizeFactor = 1)
    {
        var scaledIconRect = iconRect.ScaledBy(drawSizeFactor);
        Widgets.ThingIcon(scaledIconRect, jackpot);
        if (jackpot.stackCount > 1)
        {
            Text.Anchor = TextAnchor.LowerRight;
            Text.Font = GameFont.Tiny;
            Widgets.Label(iconRect, jackpot.stackCount.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        Text.WordWrap = true;
        if (Widgets.ButtonInvisible(iconRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(jackpot));
        }

        if (Mouse.IsOver(iconRect))
        {
            string tooltip = jackpot.Label.AsTipTitle() + "\n\n" +
                             jackpot.DescriptionDetailed;
            if (jackpot.def.useHitPoints)
            {
                tooltip = string.Concat(tooltip, "\n", jackpot.HitPoints, " / ",
                    jackpot.MaxHitPoints);
            }

            TooltipHandler.TipRegion(iconRect, tooltip);
        }
    }

    #endregion
}