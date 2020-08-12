using System.Collections.Generic;

namespace MapGeneration.Utils.GraphAnalysis.Analyzers.CycleClusters
{
    public class CycleCluster<TNode>
    {
        public List<TNode> Nodes { get; }

        public List<List<TNode>> Cycles { get; }

        public CycleCluster(List<TNode> nodes, List<List<TNode>> cycles)
        {
            Nodes = nodes;
            Cycles = cycles;
        }
    }
}