using System.Collections.Generic;
using Verse;

namespace ArchoGacha.GameComps;

public class GameComponent_GachaTracker : GameComponent
{
    public GameComponent_GachaTracker()
    {
    }

    public GameComponent_GachaTracker(Game game)
    {
    }

    //TODO: turn this into all banner types at once
    public List<Banner> banners;
    public Banner activeBanner;
    public int pitySilverReserve;

    public override void GameComponentTick()
    {
        base.GameComponentTick();
        if (GenTicks.IsTickInterval(1000))
        {
            if (activeBanner == null || activeBanner.endTicks <= Find.TickManager.TicksGame)
            {
                activeBanner = GenerateBanner();
                // Log.Message(activeBanner.ToString());
            }
            //TODO: implement cleanup and handling pity (if needed), etc
        }
    }

    public Banner GenerateBanner()
    {
        var worker = DefDatabase<PrizeGeneratorDef>.AllDefsListForReading.RandomElement().Worker;
        return new Banner(
            worker.GeneratePrize(PrizeCategory.Jackpot),
            GenerateConsolations(worker),
            //TODO: turn this into a setting
            Find.TickManager.TicksGame + 60000 * 3
        );
    }

    private static List<Thing> GenerateConsolations(PrizeWorker worker)
    {
        return new List<Thing>
        {
            worker.GeneratePrize(PrizeCategory.Consolation),
            worker.GeneratePrize(PrizeCategory.Consolation),
            worker.GeneratePrize(PrizeCategory.Consolation)
        };
    }

    public void PullOnBanner(int silverAmount)
    {
        //TODO: impl
    }
}

public class Banner
{
    public Thing jackpot;
    public List<Thing> prizes;
    public int endTicks;

    public Banner(Thing jackpot, List<Thing> prizes, int endTicks)
    {
        this.jackpot = jackpot;
        this.prizes = prizes;
        this.endTicks = endTicks;
    }

    public override string ToString()
    {
        return $"Banner{{ jackpot={jackpot}, prizes={string.Join(",", prizes)}, endTicks={endTicks} }}";
    }
}

public enum Category
{
    Weapon,
    WeaponMelee,
    WeaponRanged,
    Armor,
    Consumable,
    Bionic,
    Art,
    PawnKind,
    Animal
}