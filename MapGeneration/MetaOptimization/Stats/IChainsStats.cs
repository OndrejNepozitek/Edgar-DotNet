using System.Collections.Generic;

namespace MapGeneration.MetaOptimization.Stats
{
    public interface IChainsStats
    {
        List<ChainStats> ChainsStats { get; }
    }
}