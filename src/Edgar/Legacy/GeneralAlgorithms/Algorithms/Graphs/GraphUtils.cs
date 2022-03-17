using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;
using GraphPlanarityTesting.PlanarityTesting.BoyerMyrvold;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Graphs
{
    /// <summary>
    /// Utility methods for graphs
    /// </summary>
    public class GraphUtils
    {
        /// <summary>
        /// Checks if a given graph is connected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph">Undirected graph.</param>
        /// <returns></returns>
        public bool IsConnected<T>(IGraph<T> graph)
        {
            if (graph.IsDirected)
                throw new ArgumentException("The graph must not be directed", nameof(graph));

            // Empty graphs are connected
            if (graph.VerticesCount == 0)
                return true;

            var foundVertices = new HashSet<T>();
            var firstVertex = graph.Vertices.First();
            var queue = new Queue<T>();

            // Run a BFS from an arbitrary initial vertex
            queue.Enqueue(firstVertex);
            foundVertices.Add(firstVertex);

            while (queue.Count != 0)
            {
                var vertex = queue.Dequeue();

                foreach (var neighbour in graph.GetNeighbors(vertex))
                {
                    if (!foundVertices.Contains(neighbour))
                    {
                        foundVertices.Add(neighbour);
                        queue.Enqueue(neighbour);
                    }
                }
            }

            // The graph is connected if the number of found vertices is the same as the total number of vertices
            return graph.VerticesCount == foundVertices.Count;
        }

        /// <summary>
        /// Checks if a given graph is planar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph">Undirected graph.</param>
        /// <returns></returns>
        public bool IsPlanar<T>(IGraph<T> graph)
        {
            if (graph.IsDirected)
                throw new ArgumentException("The graph must not be directed", nameof(graph));

            if (graph.VerticesCount == 0)
                return true;

            var boyerMyrvold = new BoyerMyrvold<T>();

            return boyerMyrvold.IsPlanar(TransformGraph(graph));
        }

        /// <summary>
        /// Gets faces of a given graph.
        /// </summary>
        /// <remarks>
        /// Only distinct elements from each face are returned.
        /// </remarks>
        /// <param name="graph"></param>
        /// <returns></returns>
        public List<List<T>> GetPlanarFaces<T>(IGraph<T> graph)
        {
            if (graph.IsDirected)
                throw new ArgumentException("The graph must not be directed", nameof(graph));

            var boyerMyrvold = new BoyerMyrvold<T>();
            var isPlanar = boyerMyrvold.TryGetPlanarFaces(TransformGraph(graph), out var faces);

            if (!isPlanar)
            {
                throw new InvalidOperationException("Graph is not planar");
            }

            var facesDistinct = new List<List<T>>();

            foreach (var faceRaw in faces.Faces)
            {
                facesDistinct.Add(faceRaw.Distinct().ToList());
            }

            return facesDistinct;
        }

        /// <summary>
        /// Transforms a given graph for the planarity testing library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldGraph"></param>
        /// <returns></returns>
        private GraphPlanarityTesting.Graphs.DataStructures.IGraph<T> TransformGraph<T>(IGraph<T> oldGraph)
        {
            var newGraph = new GraphPlanarityTesting.Graphs.DataStructures.UndirectedAdjacencyListGraph<T>();

            foreach (var vertex in oldGraph.Vertices)
            {
                newGraph.AddVertex(vertex);
            }

            foreach (var edge in oldGraph.Edges)
            {
                newGraph.AddEdge(edge.From, edge.To);
            }

            return newGraph;
        }
    }
}