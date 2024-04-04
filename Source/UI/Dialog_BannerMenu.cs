using ArchoGacha.GameComps;
using UnityEngine;
using Verse;

namespace ArchoGacha.UI;

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
        // listingStandard.Gap(32f);

        if (!comp.activeBanners.NullOrEmpty())
        {
            Rect viewRect = new Rect(inRect.x, listingStandard.CurHeight, inRect.width,
                inRect.height - listingStandard.CurHeight);
            Rect scrollRect = new Rect(0f, 0f, viewRect.width - 16f,
                (Text.LineHeight + listingStandard.verticalSpacing) * comp.activeBanners.Count + 250f );


            Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect, true);
            listingStandard.ColumnWidth = scrollRect.width - 17f;
            listingStandard.Begin(scrollRect);

            var y = 0f;

            foreach (var banner in comp.activeBanners)
            {
                //TODO: finish
                Rect rect = new Rect(0f, y, scrollRect.width, 28f);
                // listingStandard.Label(banner.def.ToString());
                // Widgets.InfoCardButton(rect.width - 24f, y, thing);

                Rect rect5 = new Rect(36f, y, rect.width - 36f, rect.height);
                
                string text = banner.jackpot.LabelCap;
                Widgets.Label(rect5, text.Truncate(rect5.width, null));
                y += 28f;
                
                if (banner.jackpot.def.DrawMatSingle != null && banner.jackpot.def.DrawMatSingle.mainTexture != null)
                {
                    Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), banner.jackpot, 1f, null, false);
                }

                y += 28f;
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        listingStandard.End();
    }
}