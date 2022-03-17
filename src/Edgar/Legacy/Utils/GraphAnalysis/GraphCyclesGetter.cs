using System;
using System.Collections.Generic;
using System.Linq;
using GraphPlanarityTesting.Graphs.Algorithms;
using GraphPlanarityTesting.Graphs.DataStructures;

namespace Edgar.Legacy.Utils.GraphAnalysis
{
    public class GraphCyclesGetter<TNode>
    {
        public List<List<TNode>> GetCycles(Graphs.IGraph<TNode> graph)
        {
            var convertedGraph = new UndirectedAdjacencyListGraph<TNode>();

            foreach (var vertex in graph.Vertices)
            {
                convertedGraph.AddVertex(vertex);
            }

            foreach (var edge in graph.Edges)
            {
                convertedGraph.AddEdge(edge.From, edge.To);
            }

            var getCyclesVisitor = new GetGraphCyclesVisitor<TNode>();

            DFSTraversal.Traverse(convertedGraph, getCyclesVisitor);

            var cyclesBaseRaw = getCyclesVisitor.GetCycles();
            var edges = graph.Edges.ToList();
            var cyclesBase = cyclesBaseRaw.Select(x => new Cycle(x, edges)).ToList();
            var cycles = new List<Cycle>(cyclesBase);

            foreach (var cycle1 in cyclesBase)
            {
                foreach (var cycle2 in cyclesBase)
                {
                    if (cycle1 == cycle2)
                    {
                        continue;
                    }

                    var newEdges = new bool[edges.Count];
                    var hasSharedEdge = false;

                    for (int i = 0; i < edges.Count; i++)
                    {
                        var edge1 = cycle1.Edges[i];
                        var edge2 = cycle2.Edges[i];

                        if (edge1 && edge2)
                        {
                            hasSharedEdge = true;
                        }
                        else
                        {
                            newEdges[i] = edge1 || edge2;
                        }
                    }

                    if (hasSharedEdge)
                    {
                        if (cycles.All(x => !x.Edges.SequenceEqual(newEdges)))
                        {
                            cycles.Add(new Cycle(newEdges, edges));
                        }
                    }
                }
            }

            return cycles.Select(x => x.Nodes).ToList();
        }

        public class Cycle
        {
            public bool[] Edges { get; }

            public List<TNode> Nodes { get; }

            public Cycle(List<TNode> nodes, List<Graphs.IEdge<TNode>> orderedEdges)
            {
                Nodes = nodes;
                Edges = new bool[orderedEdges.Count];

                for (int i = 0; i < nodes.Count; i++)
                {
                    var from = nodes[i];
                    var to = nodes[(i + 1) % nodes.Count];

                    for (var j = 0; j < orderedEdges.Count; j++)
                    {
                        var edge = orderedEdges[j];

                        if ((edge.From.Equals(from) && edge.To.Equals(to)) ||
                            (edge.From.Equals(to) && edge.To.Equals(from)))
                        {
                            Edges[j] = true;
                            break;
                        }
                    }
                }
            }

            public Cycle(bool[] edges, List<Graphs.IEdge<TNode>> orderedEdges)
            {
                Edges = edges;
                Nodes = new List<TNode>();

                var edgesCount = edges.Count(x => x);
                var firstEdgeIndex = Array.FindIndex(Edges, x => x);
                var currentNode = orderedEdges[firstEdgeIndex].From;

                while (true)
                {
                    Nodes.Add(currentNode);

                    if (Nodes.Count == edgesCount)
                    {
                        break;
                    }

                    for (var i = 0; i < edges.Length; i++)
                    {
                        var isEdgeUsed = edges[i];

                        if (isEdgeUsed)
                        {
                            var edge = orderedEdges[i];

                            if (edge.From.Equals(currentNode) && !Nodes.Contains(edge.To))
                            {
                                currentNode = edge.To;
                            }
                            else if (edge.To.Equals(currentNode) && !Nodes.Contains(edge.From))
                            {
                                currentNode = edge.From;
                            }
                        }
                    }
                }
            }
        }
    }
}