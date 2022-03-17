using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;

namespace Edgar.Legacy.Utils.GraphAnalysis.Analyzers.CycleClusters
{
    public class CycleClustersAnalyzer<TNode>
    {
        public CycleClustersReport<TNode> GetReport(IGraph<TNode> graph)
        {
            var cyclesGetter = new GraphCyclesGetter<TNode>();
            var cycles = cyclesGetter.GetCycles(graph);
            var clusters = GetClusters(cycles);

            var maxDensity = 0d;

            foreach (var cluster in clusters)
            {
                if (cluster.Cycles.Count <= 3)
                {
                    continue;
                }

                var density = cluster.Cycles.Count / (double) cluster.Nodes.Count;

                if (density > maxDensity)
                {
                    maxDensity = density;
                }
            }

            return new CycleClustersReport<TNode>()
            {
                Clusters = clusters,
                MaxDensity = maxDensity,
                MaxCyclesInCluster = clusters.Count != 0 ? clusters.Max(x => x.Cycles.Count) : 0,
            };
        }

        private List<CycleCluster<TNode>> GetClusters(List<List<TNode>> cycles)
        {
            var clusters = new List<CycleCluster<TNode>>();

            while (cycles.Count != 0)
            {
                var firstCycle = cycles[cycles.Count - 1];
                cycles.RemoveAt(cycles.Count - 1);

                var clusterNodes = new List<TNode>(firstCycle);
                var clusterCycles = new List<List<TNode>>() {firstCycle};
                bool changed;
                do
                {
                    changed = false;

                    for (int i = cycles.Count - 1; i >= 0; i--)
                    {
                        var cycle = cycles[i];

                        if (clusterNodes.Intersect(cycle).Count() >= 2)
                        {
                            clusterNodes.AddRange(cycle);
                            clusterNodes = clusterNodes.Distinct().ToList();
                            clusterCycles.Add(cycle);
                            cycles.RemoveAt(i);
                            changed = true;
                        }
                    }
                } while (changed);

                clusters.Add(new CycleCluster<TNode>(clusterNodes, clusterCycles));
            }

            return clusters;
        }
    }
}