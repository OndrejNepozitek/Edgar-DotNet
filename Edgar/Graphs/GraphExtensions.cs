namespace Edgar.Graphs
{
    public static class GraphExtensions
    {
        /// <summary>
        /// Adds a range of vertices to a graph of ints.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public static void AddVerticesRange(this IGraph<int> graph, int start, int count)
        {
            for (var i = start; i < start + count; i++)
            {
                graph.AddVertex(i);
            }
        }
    }
}