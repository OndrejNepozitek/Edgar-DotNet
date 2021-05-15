using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Edgar.Graphs
{
    public static class GraphAlgorithms
    {
        public static IGraph<TNode> GetInducedSubgraph<TNode>(IGraph<TNode> originalGraph, HashSet<TNode> nodes, IGraph<TNode> newGraph)
        {
            foreach (var node in nodes)
            {
                newGraph.AddVertex(node);
            }

            foreach (var edge in originalGraph.Edges)
            {
                if (nodes.Contains(edge.From) && nodes.Contains(edge.To))
                {
                    newGraph.AddEdge(edge.From, edge.To);
                }
            }

            return newGraph;
        }

        public static List<TNode> GetShortestPath<TNode>(IGraph<TNode> graph, TNode startNode, TNode goalNode)
        {
            return GetShortestPath(graph, new List<TNode>() {startNode}, goalNode);
        }

        public static List<TNode> GetShortestPath<TNode>(IGraph<TNode> graph, List<TNode> startNodes, TNode goalNode)
        {
            if (startNodes.Any(x => x.Equals(goalNode)))
            {
                return new List<TNode>() {goalNode};
            }

            var previous = new Dictionary<TNode, TNode>();

            var queue = new Queue<TNode>();

            foreach (var startNode in startNodes)
            {
                queue.Enqueue(startNode);
            }

            while (queue.Count != 0)
            {
                var node = queue.Dequeue();

                foreach (var neighbor in graph.GetNeighbors(node))
                {
                    if (previous.ContainsKey(neighbor))
                    {
                        continue;
                    }

                    if (startNodes.Contains(neighbor))
                    {
                        continue;
                    }

                    previous[neighbor] = node;

                    if (goalNode.Equals(neighbor))
                    {
                        break;
                    }

                    queue.Enqueue(neighbor);
                }
            }

            // Check if we found any path
            if (!previous.ContainsKey(goalNode))
            {
                return null;
            }

            var path = new List<TNode>()
            {
                goalNode
            };
            var currentNode = goalNode;

            while (previous.ContainsKey(currentNode))
            {
                var previousNode = previous[currentNode];
                path.Add(previousNode);
                currentNode = previousNode;
            }

            path.Reverse();

            return path;
        }

        public static List<TNode> GetShortestMultiPath<TNode>(IGraph<TNode> graph, List<TNode> startNodes)
        {
            if (startNodes.Count < 2 || startNodes.Count > 3)
            {
                throw new InvalidOperationException("There must be between 2 and 3 nodes.");
            }

            if (startNodes.Count == 2)
            {
                return GetShortestPath(graph, startNodes[0], startNodes[1]);
            }

            var bestPath = default(List<TNode>);

            foreach (var middleNode in graph.Vertices.ToList())
            {
                var path = new List<TNode>();

                foreach (var startNode in startNodes)
                {
                    var startMiddlePath = GetShortestPath(graph, startNode, middleNode);

                    if (startMiddlePath == null)
                    {
                        return null;
                    }

                    path.AddRange(startMiddlePath);
                }

                path = path.Distinct().ToList();

                if (bestPath == null || path.Count < bestPath.Count)
                {
                    bestPath = path;
                }
            }

            return bestPath;
        }
    }
}