using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public static class ChainDecompositionGraphUtils
    {
        public static Dictionary<TNode, bool> MakeBorderNodesWalkable<TNode>(IGraph<TNode> graph,
            Dictionary<TNode, bool> isWalkable)
        {
            var isWalkableNew = new Dictionary<TNode, bool>(isWalkable);

            foreach (var node in graph.Vertices)
            {
                if (isWalkable[node] == false && graph.GetNeighbors(node).Any(x => isWalkable[x]))
                {
                    isWalkableNew[node] = true;
                }
            }

            return isWalkableNew;
        }

        public static List<List<TNode>> GetWalkableComponents<TNode>(IGraph<TNode> graph,
            Dictionary<TNode, bool> isWalkable)
        {
            var visited = new HashSet<TNode>();
            var components = new List<List<TNode>>();

            foreach (var node in graph.Vertices)
            {
                if (visited.Contains(node) == false)
                {
                    var component = GetReachableNodes(graph, node, isWalkable);

                    if (component.Count != 0)
                    {
                        components.Add(component);
                        component.ForEach(x => visited.Add(x));
                    }
                }
            }

            return components;
        }

        public static List<TNode> GetReachableNodes<TNode>(IGraph<TNode> graph, TNode startNode,
            Dictionary<TNode, bool> isWalkable)
        {
            if (!isWalkable[startNode])
            {
                return new List<TNode>();
            }

            var visited = new HashSet<TNode>();
            var queue = new Queue<TNode>();
            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count != 0)
            {
                var node = queue.Dequeue();

                foreach (var neighbor in graph.GetNeighbors(node))
                {
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!isWalkable[neighbor])
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return visited.ToList();
        }
    }
}