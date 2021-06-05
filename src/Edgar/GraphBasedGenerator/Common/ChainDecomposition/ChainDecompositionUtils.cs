using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public static class ChainDecompositionUtils
    {
        public static List<TNode> GetInitialFixedComponent<TNode>(IGraph<TNode> graph, List<TNode> fixedRooms)
        {
            if (fixedRooms == null)
            {
                return null;
            }

            // TODO: this is inefficient
            var relevantFixedRooms = graph.Vertices.Where(x => fixedRooms.Contains(x)).ToList();

            if (relevantFixedRooms.Count == 0)
            {
                return null;
            }

            var chain = GraphAlgorithms.GetShortestMultiPath(graph, relevantFixedRooms);
            chain = GraphAlgorithms.OrderNodesByDFSDistance(graph, chain);

            return chain;
        }

        public static ChainCandidate<TNode> GetBfsTreeCandidate<TNode>(PartialDecomposition<TNode> decomposition, List<TNode> startingNodes, int maxTreeSize)
        {
            var nodes = new List<TNode>();
            var queue = new Queue<TNode>();

            nodes.AddRange(startingNodes);
            foreach (var startingNode in startingNodes)
            {
                queue.Enqueue(startingNode);
            }

            while (queue.Count != 0 && nodes.Count < maxTreeSize)
            {
                var node = queue.Dequeue();

                if (decomposition.GetRemainingFaces().Any(x => x.Contains(node)))
                {
                    continue;
                }

                var neighbors = decomposition.Graph.GetNeighbors(node);

                foreach (var neighbor in neighbors)
                {
                    if (!nodes.Contains(neighbor) && !decomposition.IsCovered(neighbor))
                    {
                        nodes.Add(neighbor);
                        queue.Enqueue(neighbor);

                        if (nodes.Count >= maxTreeSize)
                        {
                            break;
                        }
                    }
                }
            }

            return new ChainCandidate<TNode>()
            {
                Nodes = nodes,
                IsFromFace = false,
                MinimumNeighborChainNumber = GetMinimumNeighborChainNumber(decomposition, nodes),
            };
        }

        public static ChainCandidate<TNode> GetDfsTreeCandidate<TNode>(PartialDecomposition<TNode> decomposition, List<TNode> startingNodes, int maxTreeSize)
        {
            var nodes = new List<TNode>();
            var stack = new Stack<TNode>();

            nodes.AddRange(startingNodes);
            foreach (var startingNode in startingNodes)
            {
                stack.Push(startingNode);
            }

            while (stack.Count != 0 && nodes.Count < maxTreeSize)
            {
                var node = stack.Pop();

                if (decomposition.GetRemainingFaces().Any(x => x.Contains(node)))
                {
                    continue;
                }

                var neighbors = decomposition.Graph.GetNeighbors(node);

                foreach (var neighbor in neighbors)
                {
                    if (!nodes.Contains(neighbor) && !decomposition.IsCovered(neighbor))
                    {
                        nodes.Add(neighbor);
                        stack.Push(neighbor);

                        if (nodes.Count >= maxTreeSize)
                        {
                            break;
                        }
                    }
                }
            }

            return new ChainCandidate<TNode>()
            {
                Nodes = nodes,
                IsFromFace = false,
                MinimumNeighborChainNumber = GetMinimumNeighborChainNumber(decomposition, nodes),
            };
        }

        public static int GetMinimumNeighborChainNumber<TNode>(PartialDecomposition<TNode> decomposition, List<TNode> nodes)
        {
            var coveredNeighbors = nodes
                .SelectMany(decomposition.Graph.GetNeighbors)
                .Where(decomposition.IsCovered)
                .ToList();

            if (coveredNeighbors.Count != 0)
            {
                return coveredNeighbors.Min(decomposition.GetChainNumber);
            }

            return -1;
        }

        public static ChainCandidate<TNode> GetCycleComponent<TNode>(PartialDecomposition<TNode> decomposition, List<TNode> face)
        {
            var nodes = new List<TNode>();
            var notCoveredNodes = face.Where(x => !decomposition.IsCovered(x)).ToList();
            var nodeOrder = new Dictionary<TNode, int>();

            while (notCoveredNodes.Count != 0)
            {
                var nodeIndex = notCoveredNodes
                    .MinBy(
                        x => decomposition.Graph
                            .GetNeighbors(x)
                            .Min(y =>
                                decomposition.IsCovered(y)
                                    ? -1
                                    : nodeOrder.ContainsKey(y) ? nodeOrder[y] : int.MaxValue));

                nodeOrder[notCoveredNodes[nodeIndex]] = nodeOrder.Count;
                nodes.Add(notCoveredNodes[nodeIndex]);
                notCoveredNodes.RemoveAt(nodeIndex);
            }

            return new ChainCandidate<TNode>()
            {
                Nodes = nodes,
                IsFromFace = true,
                MinimumNeighborChainNumber = ChainDecompositionUtils.GetMinimumNeighborChainNumber(decomposition, nodes),
            };
        }
    }
}