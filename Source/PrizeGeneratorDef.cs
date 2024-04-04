using System;
using Verse;

namespace ArchoGacha;

public class PrizeGeneratorDef : Def
{
    public Type prizeWorkerClass = typeof(PrizeWorker);

    [Unsaved] private PrizeWorker workerInt;

    //TODO: change price logic to be if(banner min > settingsMin) use settings min 
    public float minJackpotMarketValue;
    public float minConsolationMarketValue;

    public PrizeWorker Worker
    {
        get
        {
            if (workerInt != null && prizeWorkerClass != null) return workerInt;

            workerInt = (PrizeWorker)Activator.CreateInstance(prizeWorkerClass);
            workerInt.def = this;
            return workerInt;
        }
    }
}