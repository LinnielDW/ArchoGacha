using System.Linq;
using ArchoGacha.Utils;

namespace ArchoGacha.PrizeBannerClasses;

public class PawnPrizeBanner : PrizeBanner
{
    public override void CalculatePullPrice()
    {
        pullPrice = (int)Math.Ceiling(PawnValue((Pawn)jackpot) *
                                      settings.jackpotChance *
                                      settings.pullPriceFactor *
                                      jackpot.stackCount);
    }

    protected override Thing SelectPrizeDef(PrizeCategory prizeCategory,
        float valueMaxOverride = 0,
        bool excludeJackpotDef = false)
    {
        PawnKindDef kind;
        if (prizeCategory == PrizeCategory.Jackpot)
        {
            var validKinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(pk =>
                pk.RaceProps.Humanlike &&
                pk is not CreepJoinerFormKindDef &&
                pk.apparelMoney.TrueMin + pk.weaponMoney.TrueMin >
                Math.Max(settings.minJackpotOffset, def.minJackpotMarketValue)).ToList();
            // Log.Message(validKinds.ToStringSafeEnumerable());
            kind = validKinds.RandomElement();
        }
        else
        {
            kind = PawnKindDefOf.WildMan;
        }

        var request = new PawnGenerationRequest(kind, null,
            PawnGenerationContext.NonPlayer, -1, true, false, false, false,
            prizeCategory == PrizeCategory.Jackpot, 0f, false, true, true, true, true,
            false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null,
            null, null, null, null, null, null, null, false, false, false, false, null,
            null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, true);

        var returnPawn = PawnGenerator.GeneratePawn(request);

        if (prizeCategory == PrizeCategory.Jackpot && PawnValue(returnPawn) <
            Math.Max(settings.minJackpotOffset, def.minJackpotMarketValue))
        {
            // Log.Message($"Pawn value ${PawnValue(returnPawn)} less than required, upgrading pawn gear.");
            var equipment =
                returnPawn.equipment.AllEquipmentListForReading.Concat(returnPawn.apparel
                    .WornApparel);
            foreach (var eq in equipment.Where(t => t.HasComp<CompQuality>()))
            {
                // Log.Message(eq.Label);
                eq.GetComp<CompQuality>().SetQuality(
                    QualityUtility.GenerateQualitySuper(), ArtGenerationContext.Outsider);
                if (PawnValue(returnPawn) >= Math.Max(settings.minJackpotOffset,
                        def.minJackpotMarketValue))
                {
                    break;
                }
            }
            // Log.Message("value now: " + PawnValue(returnPawn));
        }

        if (ModsConfig.IdeologyActive)
        {
            if (Rand.Chance(0.5f))
            {
                returnPawn.ideo?.SetIdeo(Faction.OfPlayer.ideos.PrimaryIdeo);
            }
        }

        return returnPawn;
    }

    private static float PawnValue(Pawn pawn)
    {
        return pawn.MarketValue + pawn.apparel.WornApparel.Sum(t => t.MarketValue) +
               pawn.equipment.AllEquipmentListForReading.Sum(t => t.MarketValue);
    }
}