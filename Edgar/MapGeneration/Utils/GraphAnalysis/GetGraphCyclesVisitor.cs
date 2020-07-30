using System.Collections.Generic;
using GraphPlanarityTesting.Graphs.Algorithms;
using GraphPlanarityTesting.Graphs.DataStructures;

namespace MapGeneration.Utils.GraphAnalysis
{
    public class GetGraphCyclesVisitor<TNode> : BaseDFSTraversalVisitor<TNode>
    {
        private readonly Dictionary<TNode, TNode> predecessors = new Dictionary<TNode, TNode>();
        private readonly List<List<TNode>> cycles = new List<List<TNode>>();

        public override void TreeEdge(IEdge<TNode> edge, IGraph<TNode> graph)
        {
            predecessors[edge.Target] = edge.Source;
        }

        public override void BackEdge(IEdge<TNode> edge, IGraph<TNode> graph)
        {
            if (predecessors[edge.Source].Equals(edge.Target))
            {
                return;
            }

            var cycle = new List<TNode>();
            var currentNode = edge.Source;

            while (!currentNode.Equals(edge.Target))
            {
                cycle.Add(currentNode);
                currentNode = predecessors[currentNode];
            }

            cycle.Add(edge.Target);

            cycles.Add(cycle);
        }

        public List<List<TNode>> GetCycles()
        {
            return cycles;
        }
    }
}