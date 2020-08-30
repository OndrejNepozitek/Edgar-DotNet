using System.Collections.Generic;
using Edgar.Legacy.Benchmarks.Interfaces;

namespace Edgar.Legacy.Utils.PerformanceAnalysis
{
    public class RunInfo
    {
        public List<ChainInfo> ChainInfos { get; set; }

        public IGeneratorRun Run { get; set; }
    }
}