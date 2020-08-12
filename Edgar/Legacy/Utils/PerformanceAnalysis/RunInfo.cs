using System.Collections.Generic;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.Interfaces;

namespace MapGeneration.Utils.PerformanceAnalysis
{
    public class RunInfo
    {
        public List<ChainInfo> ChainInfos { get; set; }

        public IGeneratorRun Run { get; set; }
    }
}