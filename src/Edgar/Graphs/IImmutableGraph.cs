using System.Collections.Immutable;

namespace Edgar.Graphs
{
    /// <summary>
    /// Immutable graph interface with faster access to data through ImmutableArray.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IImmutableGraph<T> : IGraph<T>
    {
        /// <summary>
        /// Gets all vertices of the graph.
        /// </summary>
        new ImmutableArray<T> Vertices { get; }

        /// <summary>
        /// Gets all edges of the graph.
        /// </summary>
        new ImmutableArray<IEdge<T>> Edges { get; }

        /// <summary>
        /// Gets all neighbors of a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        new ImmutableArray<T> GetNeighbors(T vertex);
    }
}