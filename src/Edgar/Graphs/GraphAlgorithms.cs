using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Utils;

namespace Edgar.Graphs
{
    public static class GraphAlgorithms
    {
        public static List<TNode> OrderNodesByDFSDistance<TNode>(IGraph<TNode> graph, List<TNode> startingNodes,
            List<TNode> nodes = null)
        {
            nodes = nodes ?? graph.Vertices.ToList();
            var remainingNodes = nodes.ToHashSet();
            var result = new List<TNode>();
            var queue = new Queue<TNode>();

            foreach (var node in startingNodes)
            {
                queue.Enqueue(node);
                result.Add(node);
                remainingNodes.Remove(node);
            }

            while (queue.Count != 0)
            {
                var node = queue.Dequeue();

                foreach (var neighbor in graph.GetNeighbors(node))
                {
                    if (remainingNodes.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        result.Add(neighbor);
                        remainingNodes.Remove(neighbor);
                    }
                }
            }

            return result;
        }

        public static IGraph<TNode> GetInducedSubgraph<TNode>(IGraph<TNode> originalGraph, HashSet<TNode> nodes,
            IGraph<TNode> newGraph)
        {
            var originalVertices = originalGraph.Vertices.ToHashSet();

            foreach (var node in nodes.Where(x => originalVertices.Contains(x)))
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
            if (startNodes.Count < 1 || startNodes.Count > 3)
            {
                throw new InvalidOperationException("There must be between 1 and 3 start nodes.");
            }

            if (startNodes.Count == 1)
            {
                return new List<TNode>() {startNodes[0]};
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

        public static List<IEdge<TNode>> FindBridges<TNode>(IGraph<TNode> graph)
        {
            var timer = 0;
            var visited = new Dictionary<TNode, bool>();
            var timeIn = new Dictionary<TNode, int>();
            var low = new Dictionary<TNode, int>();

            foreach (var vertex in graph.Vertices)
            {
                visited[vertex] = false;
                timeIn[vertex] = -1;
                low[vertex] = -1;
            }

            var bridges = new List<IEdge<TNode>>();

            foreach (var vertex in graph.Vertices)
            {
                if (!visited[vertex])
                {
                    FindBridgesDfs(graph, vertex, default(TNode), true, visited, timeIn, low, ref timer, bridges);
                }
            }

            return bridges;
        }

        private static void FindBridgesDfs<TNode>(IGraph<TNode> graph, TNode vertex, TNode parent, bool isRoot,
            Dictionary<TNode, bool> visited, Dictionary<TNode, int> timeIn, Dictionary<TNode, int> low, ref int timer,
            List<IEdge<TNode>> bridges)
        {
            visited[vertex] = true;
            var time = timer++;
            timeIn[vertex] = time;
            low[vertex] = time;

            foreach (var neighbor in graph.GetNeighbors(vertex))
            {
                if (!isRoot && neighbor.Equals(parent))
                {
                    continue;
                }

                if (visited[neighbor])
                {
                    low[vertex] = Math.Min(low[vertex], timeIn[neighbor]);
                }
                else
                {
                    FindBridgesDfs(graph, neighbor, vertex, false, visited, timeIn, low, ref timer, bridges);
                    low[vertex] = Math.Min(low[vertex], low[neighbor]);

                    if (low[neighbor] > timeIn[vertex])
                    {
                        bridges.Add(new Edge<TNode>(neighbor, vertex));
                    }
                }
            }
        }
    }
}