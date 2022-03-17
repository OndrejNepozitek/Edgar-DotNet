using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Edgar.Graphs
{
    /// <summary>
    /// Undirected immutable graph.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndirectedImmutableGraph<T> : IImmutableGraph<T>
    {
        /// <inheritdoc />
        public bool IsDirected { get; } = false;

        IEnumerable<T> IGraph<T>.Vertices => Vertices;

        IEnumerable<IEdge<T>> IGraph<T>.Edges => Edges;

        /// <inheritdoc />
        public ImmutableArray<T> Vertices { get; }

        /// <inheritdoc />
        public ImmutableArray<IEdge<T>> Edges { get; }

        /// <inheritdoc />
        public int VerticesCount => Vertices.Length;

        private readonly Dictionary<T, ImmutableArray<T>> adjacencyLists;

        private readonly HashSet<Edge<T>> edges;

        public UndirectedImmutableGraph(IGraph<T> graph)
        {
            Vertices = ImmutableArray.CreateRange(graph.Vertices);
            Edges = ImmutableArray.CreateRange(graph.Edges);
            adjacencyLists = new Dictionary<T, ImmutableArray<T>>();
            edges = new HashSet<Edge<T>>();

            foreach (var vertex in Vertices)
            {
                adjacencyLists[vertex] = ImmutableArray.CreateRange(graph.GetNeighbors(vertex));
            }

            foreach (var edge in Edges)
            {
                edges.Add(new Edge<T>(edge.From, edge.To));
            }
        }

        /// <inheritdoc />
        public void AddVertex(T vertex)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void AddEdge(T from, T to)
        {
            throw new NotSupportedException();
        }

        ImmutableArray<T> IImmutableGraph<T>.GetNeighbors(T vertex)
        {
            if (!adjacencyLists.TryGetValue(vertex, out var neighbors))
                throw new ArgumentException("The vertex does not exist");

            return neighbors;
        }

        public IEnumerable<T> GetNeighbours(T vertex)
        {
            return GetNeighbors(vertex);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetNeighbors(T vertex)
        {
            return ((IImmutableGraph<T>) this).GetNeighbors(vertex);
        }

        /// <inheritdoc />
        public bool HasEdge(T from, T to)
        {
            foreach (var neighbor in ((IImmutableGraph<T>) this).GetNeighbors(from))
            {
                if (neighbor.Equals(to))
                    return true;
            }

            return false;
        }
    }
}