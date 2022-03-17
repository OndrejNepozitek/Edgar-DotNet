namespace Edgar.Graphs
{
    /// <summary>
    /// Interface describing an edge of a graph.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEdge<T>
    {
        /// <summary>
        /// First vertex of the edge.
        /// </summary>
        T From { get; }

        /// <summary>
        /// Second vertex of the edge.
        /// </summary>
        T To { get; }
    }
}