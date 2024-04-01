using System;
using Verse;

namespace ArchoGacha;

public class PrizeGeneratorDef : Def
{
    public Type prizeWorkerClass = typeof(PrizeWorker);

    [Unsaved] private PrizeWorker workerInt;

    public float minJackpotMarketValue;
    public float minConsolationMarketValue;
    public PrizeWorker Worker
    {
        get
        {
            if (workerInt != null) return workerInt;

            workerInt = (PrizeWorker)Activator.CreateInstance(prizeWorkerClass);
            workerInt.def = this;
            return workerInt;
        }
    }
}