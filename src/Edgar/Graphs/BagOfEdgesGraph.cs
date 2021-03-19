using System;
using System.Collections.Generic;

namespace Edgar.Graphs
{
    /// <summary>
    /// Fast check if an edge exists, slow retrieval of neighbors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BagOfEdgesGraph<T> : IGraph<T>
    {
        public bool IsDirected => true;

        public IEnumerable<T> Vertices => vertices;

        public IEnumerable<IEdge<T>> Edges => edges;

        public int VerticesCount => vertices.Count;

        private readonly HashSet<T> vertices = new HashSet<T>();

        private readonly HashSet<Edge<T>> edges = new HashSet<Edge<T>>();

        public void AddVertex(T vertex)
        {
            vertices.Add(vertex);
        }

        public void AddEdge(T @from, T to)
        {
            edges.Add(new Edge<T>(from, to));
        }

        public IEnumerable<T> GetNeighbours(T vertex)
        {
            foreach (var edge in edges)
            {
                if (edge.From.Equals(vertex))
                {
                    yield return edge.To;
                } 
                else if (edge.To.Equals(vertex))
                {
                    yield return edge.From;
                }
            }
        }

        public bool HasEdge(T @from, T to)
        {
            return edges.Contains(new Edge<T>(from, to));
        }
    }
}