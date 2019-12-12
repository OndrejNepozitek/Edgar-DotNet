using System.Collections.Generic;
using MapGeneration.Benchmarks;
using MapGeneration.Interfaces.Benchmarks;

namespace MapGeneration.Utils.PerformanceAnalysis
{
    public class RunInfo
    {
        public List<ChainInfo> ChainInfos { get; set; }

        public IGeneratorRun Run { get; set; }
    }
}