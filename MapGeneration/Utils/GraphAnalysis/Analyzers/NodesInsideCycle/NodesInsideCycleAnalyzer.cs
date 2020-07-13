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
                    var threshold = 1;

                    if (HasNonCycleNodes(graph, commonNodes) >= threshold) positiveCount++;
                    if (HasNonCycleNodes(graph, cycle1Only) >= threshold) positiveCount++;
                    if (HasNonCycleNodes(graph, cycle2Only) >= threshold) positiveCount++;

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

        private int HasNonCycleNodes(IGraph<TNode> graph, List<TNode> nodes)
        {
            var startingNodes = new List<TNode>();
            var nonCycleNodes = new List<TNode>();
            
            foreach (var node in nodes)
            {
                var neighbors = graph.GetNeighbours(node).ToList();

                // We need only nodes that are not on the border
                if (neighbors.Intersect(nodes).Count() < 2)
                {
                    continue;
                }

                startingNodes.Add(node);

                //if (neighbors.Count > 2)
                //{
                //    return true;
                //}
            }

            var buffer = new Stack<TNode>(startingNodes);

            while (buffer.Count != 0)
            {
                var node = buffer.Pop();

                foreach (var neighbor in graph.GetNeighbours(node))
                {
                    if (!nodes.Contains(neighbor) && !nonCycleNodes.Contains(neighbor))
                    {
                        nonCycleNodes.Add(neighbor);
                        buffer.Push(neighbor);
                    }
                }
            }

            return nonCycleNodes.Count;
        }
    }
}