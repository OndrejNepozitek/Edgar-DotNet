using System.Collections.Generic;

namespace Edgar.Legacy.Utils.MetaOptimization.Stats
{
    public interface IChainsStats
    {
        List<ChainStats> ChainsStats { get; }
    }
}