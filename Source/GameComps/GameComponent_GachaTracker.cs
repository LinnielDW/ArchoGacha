using System.Collections.Generic;
using Verse;

namespace ArchoGacha.GameComps;

public class GameComponent_GachaTracker : GameComponent
{
    public List<Banner> banners;
    public Banner activeBanner;
    public int pitySilverReserve;

    public override void GameComponentTick()
    {
        if (Find.TickManager.TicksGame % 1000 == 0)
        {
            //TODO: check if banner has ended, if so, generate new banner
        }
    }

    public Banner GenerateBanner()
    {
        var worker = DefDatabase<PrizeGeneratorDef>.AllDefsListForReading.RandomElement().Worker;
        return new Banner(
            worker.GeneratePrize(PrizeCategory.Jackpot),
            GenerateConsolations(worker),
            Find.TickManager.TicksGame
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
    public int startTicksGame;

    public Banner(Thing jackpot, List<Thing> prizes, int startTicksGame)
    {
        this.jackpot = jackpot;
        this.prizes = prizes;
        this.startTicksGame = startTicksGame;
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