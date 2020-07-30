using System.Collections.Generic;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class GeneratorData : IChainsStats, IBasicStats
    {
        public List<ChainStats> ChainsStats { get; set; }

        public double Time { get; set; }

        public double Iterations { get; set; }

        public GeneratorData(List<ChainStats> chainsStats)
        {
            ChainsStats = chainsStats;
        }

        public GeneratorData()
        {
            ChainsStats = new List<ChainStats>();
        }
    }
}