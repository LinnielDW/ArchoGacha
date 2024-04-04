﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ArchoGacha.GameComps;

public class GameComponent_GachaTracker : GameComponent
{
    public static GameComponent_GachaTracker Instance;
    public GameComponent_GachaTracker()
    {
        Instance = this;
    }

    public GameComponent_GachaTracker(Game game)
    {
        Instance = this;
    }

    public List<Banner> activeBanners;
    public int pitySilverReserve;
    public int bannersEndTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref activeBanners, "activeBanners", LookMode.Deep, Array.Empty<object>());
        Scribe_Values.Look(ref pitySilverReserve, "pitySilverReserve");
        Scribe_Values.Look(ref bannersEndTick, "bannersEndTick");
    }

    public override void GameComponentTick()
    {
        base.GameComponentTick();
        if (GenTicks.IsTickInterval(1000))
        {
            if (activeBanners.NullOrEmpty() || bannersEndTick <= Find.TickManager.TicksGame)
            {
                activeBanners = DefDatabase<PrizeGeneratorDef>.AllDefsListForReading
                    .Select(GenerateBannerFromDef).ToList();

                bannersEndTick = (int)(Find.TickManager.TicksGame + 60000 * ArchoGachaSettings.bannerDurationDays);
                
                foreach (var banner in activeBanners)
                {
                    Log.Message(banner.ToString());
                }
            }
            //TODO: implement cleanup and handling pity (if needed), etc
        }
    }

    public Banner GenerateRandomBanner()
    {
        return GenerateBannerFromDef(DefDatabase<PrizeGeneratorDef>.AllDefsListForReading.RandomElement());
    }

    public Banner GenerateBannerFromDef(PrizeGeneratorDef def)
    {
        var worker = def.Worker;
        return new Banner(
            worker.GeneratePrize(PrizeCategory.Jackpot),
            GenerateConsolations(worker)
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

public class Banner : IExposable
{
    public Thing jackpot;
    public List<Thing> prizes;

    public Banner(Thing jackpot, List<Thing> prizes)
    {
        this.jackpot = jackpot;
        this.prizes = prizes;
    }

    public override string ToString()
    {
        return $"Banner{{ jackpot={jackpot}, prizes={string.Join(",", prizes)} }}";
    }

    public void ExposeData()
    {
        Scribe_References.Look(ref this.jackpot, "jackpot", false);
        Scribe_Collections.Look(ref this.prizes, "prizes",LookMode.Reference);
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