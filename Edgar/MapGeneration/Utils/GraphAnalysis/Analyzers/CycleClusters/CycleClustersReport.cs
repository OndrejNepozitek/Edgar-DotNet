using System.Collections.Generic;

namespace MapGeneration.Utils.GraphAnalysis.Analyzers.CycleClusters
{
    public class CycleClustersReport<TNode>
    {
        public List<CycleCluster<TNode>> Clusters { get; set; }

        public double MaxDensity { get; set; }

        public int MaxCyclesInCluster { get; set; }
    }
}