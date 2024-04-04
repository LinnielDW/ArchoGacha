using System.Collections.Generic;
using Verse;

namespace ArchoGacha.GameComps;

public class Banner : IExposable
{
    public Thing jackpot;
    public List<Thing> prizes;
    public PrizeGeneratorDef def;

    public Banner()
    {
    }

    public Banner(Thing jackpot, List<Thing> prizes, PrizeGeneratorDef def)
    {
        this.jackpot = jackpot;
        this.prizes = prizes;
        this.def = def;
    }

    public override string ToString()
    {
        return
            $"{def.LabelCap} {{ jackpot={jackpot}, prizes={string.Join(",", prizes)}, def={def} }}";
    }

    public void ExposeData()
    {
        Scribe_Deep.Look(ref jackpot, true, "jackpot");
        Scribe_Collections.Look(ref prizes, "prizes", true, LookMode.Deep);
        Scribe_Defs.Look(ref def, "def");
    }
}