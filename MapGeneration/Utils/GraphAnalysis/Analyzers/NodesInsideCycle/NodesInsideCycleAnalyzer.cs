using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;

namespace MapGeneration.Utils.GraphAnalysis.Analyzers.NodesInsideCycle
{
    public class NodesInsideCycleAnalyzer<TNode>
    {
        public NodesInsideCycleReport<TNode> GetReport(IGraph<TNode> graph)
        {
            var cyclesGetter = new GraphCyclesGetter<TNode>();
            var cycles = cyclesGetter.GetCycles(graph);
            var problemsCount = 0;

            foreach (var cycle1 in cycles)
            {
                foreach (var cycle2 in cycles)
                {
                    if (cycle1 == cycle2)
                    {
                        continue;
                    }

                    var commonNodes = cycle1.Intersect(cycle2).ToList();
                    var cycle1Only = cycle1.Except(commonNodes).ToList();
                    var cycle2Only = cycle2.Except(commonNodes).ToList();

                    if (commonNodes.Count < 3)
                    {
                        continue;
                    }

                    var positiveCount = 0;

                    if (HasNonCycleNodes(graph, commonNodes)) positiveCount++;
                    if (HasNonCycleNodes(graph, cycle1Only)) positiveCount++;
                    if (HasNonCycleNodes(graph, cycle2Only)) positiveCount++;

                    if (positiveCount >= 2)
                    {
                        problemsCount++;
                    }
                }
            }

            return new NodesInsideCycleReport<TNode>()
            {
                ProblemsCount = problemsCount,
            };
        }

        private bool HasNonCycleNodes(IGraph<TNode> graph, List<TNode> nodes)
        {
            foreach (var node in nodes)
            {
                var neighbors = graph.GetNeighbours(node).ToList();

                // We need only nodes that are not on the border
                if (neighbors.Intersect(nodes).Count() < 2)
                {
                    continue;
                }

                if (neighbors.Count > 2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}