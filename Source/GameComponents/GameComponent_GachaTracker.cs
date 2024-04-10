using System.Collections.Generic;
using System.Linq;
using ArchoGacha.Defs;
using ArchoGacha.UI;
using ArchoGacha.Utils;
using static ArchoGacha.GameComponents.PrizePickerUtil;

namespace ArchoGacha.GameComponents;

public class GameComponent_GachaTracker : GameComponent, ICommunicable
{
    public List<PrizeBanner> activeBanners = new();
    public float pitySilverReserve;
    public int bannersEndTick;
    public bool lostFiftyFifty;

    public List<ThingDef> trash = new();

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        
        trash = DefDatabase<ThingDef>.AllDefs.Where(d =>
            d.category == ThingCategory.Item &&
            d.tradeability.TraderCanSell() &&
            d.equipmentType == EquipmentType.None &&
            d.BaseMarketValue >= 1f &&
            !d.HasComp(typeof(CompHatcher))
        ).ToList();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref activeBanners, "activeBanners", true,
            LookMode.Deep);
        Scribe_Values.Look(ref pitySilverReserve, "pitySilverReserve");
        Scribe_Values.Look(ref bannersEndTick, "bannersEndTick");
        Scribe_Values.Look(ref lostFiftyFifty, "lostFiftyFifty");
    }

    public override void GameComponentTick()
    {
        base.GameComponentTick();
        if (GenTicks.IsTickInterval(1000))
        {
            if (activeBanners.NullOrEmpty() ||
                bannersEndTick <= Find.TickManager.TicksGame)
            {
                GenerateActiveBanners();
            }
        }
    }

    public void GenerateActiveBanners()
    {
        activeBanners.Clear();
        Dialog_BannerMenu.selectedBanner = null;

        if (settings.limitBanners && DefDatabase<PrizeBannerDef>.AllDefsListForReading.Sum(b => b.spawnCountRange.max) > settings.bannerLimit)
        {
            while (activeBanners.Count < settings.bannerLimit)
            {
                var banner = DefDatabase<PrizeBannerDef>.AllDefsListForReading.Where(def => def.spawnCountRange.max > activeBanners.Count(activeBanner => activeBanner.def == def))
                    .RandomElementByWeight(d => d.selectionWeight);
                activeBanners.Add(BannerMaker.GenerateBannerFromDef(banner));
            }
        }
        else
        {
            foreach (var b in DefDatabase<PrizeBannerDef>.AllDefsListForReading)
            {
                for (var i = 0; i < b.spawnCountRange.RandomInRange; i++)
                {
                    activeBanners.Add(BannerMaker.GenerateBannerFromDef(b));
                }
            }
        }
        activeBanners.SortByDescending(b => b.def.displayPriority);

        bannersEndTick = Find.TickManager.TicksGame +
                         (int)(60000f * settings.bannerDurationDays);

        Find.LetterStack.ReceiveLetter(
            "ArchoGacha_BannersRefreshedLabel".Translate(),
            "ArchoGacha_BannersRefreshed".Translate(),
            LetterDefOf.NeutralEvent);

        //TODO: add debug info setting
        /*foreach (var banner in activeBanners)
        {
            Log.Message(banner.ToString());
        }*/
    }


    public void PullOnBanner(PrizeBanner prizeBanner)
    {
        if (CanPullOnBanner(prizeBanner))
        {
            DoPull(prizeBanner);
        }
    }

    public void PullTenOnBanner(PrizeBanner prizeBanner)
    {
        if (CanPullTenOnBanner(prizeBanner))
        {
            for (int i = 0; i < 10; i++)
            {
                DoPull(prizeBanner);
            }
        }
    }

    private void DoPull(PrizeBanner prizeBanner)
    {
        TradeUtility.LaunchSilver(Find.CurrentMap, (int)prizeBanner.pullPrice);
        pitySilverReserve += prizeBanner.pullPrice;
        var prize = this.DecidePrize(prizeBanner);

        DeliverPrize(prize);
    }

    public bool CanPullOnBanner(PrizeBanner prizeBanner)
    {
        return prizeBanner.CanPull(Find.CurrentMap) ||
               Prefs.DevMode && settings.debugAlwaysPullable;
    }    
    public bool CanPullTenOnBanner(PrizeBanner prizeBanner)
    {
        return prizeBanner.CanPullTen(Find.CurrentMap) ||
               Prefs.DevMode && settings.debugAlwaysPullable;
    }

    public void DeliverPrize(Thing prize)
    {
        var mPrize = prize.TryMakeMinified();
        TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(Find.CurrentMap), Find.CurrentMap,
            mPrize);
    }

    public GameComponent_GachaTracker(): base()
    {
    }
    public GameComponent_GachaTracker(Game game)
    {
    }

    public string GetCallLabel()
    {
        return "ArchoGacha_CallLabel".Translate();
    }

    public string GetInfoText()
    {
        return "ArchoGacha_CallText".Translate();
    }

    public void TryOpenComms(Pawn negotiator)
    {
        Find.WindowStack.Add(new Dialog_BannerMenu(this));
    }

    public Faction GetFaction()
    {
        return null;
    }

    public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console,
        Pawn negotiator)
    {
        return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(GetCallLabel(), delegate
        {
            console.GiveUseCommsJob(negotiator, this);
        }, MenuOptionPriority.InitiateSocial), negotiator, console);
    }
}