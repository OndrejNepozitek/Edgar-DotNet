namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;

	/// <inheritdoc />
	/// <summary>
	/// Class representing a graph of ints which is in fact an alias for a generic graph.
	/// </summary>
	/// <remarks>
	/// The idea is to provide an integer interface that may save some time because the vertices
	/// can be used directly as indices to an array instead of using something like dictionary for
	/// storing values.
	/// </remarks>
	/// <typeparam name="T">Generic type of the original graph.</typeparam>
	public class IntGraph<T> : IGraph<int>
	{
		/// <inheritdoc />
		public bool IsDirected => graphAlias.IsDirected;

		/// <inheritdoc />
		public IEnumerable<int> Vertices => graphAlias.Vertices;

		/// <inheritdoc />
		public IEnumerable<IEdge<int>> Edges => graphAlias.Edges;

		/// <inheritdoc />
		public int VerticesCount => graphAlias.VerticesCount;

		private Dictionary<T, int> forwardMap;

		private List<T> reverseMap;

		private readonly IGraph<int> graphAlias;

		/// <summary>
		/// Creates a graph which will use a graph returned from a given graphCreator as
		/// an underlying data structure.
		/// </summary>
		/// <param name="graphCreator">Returns an instance of a graph that will be used as an underlying data structure.</param>
		public IntGraph(Func<IGraph<int>> graphCreator)
		{
			reverseMap = new List<T>();
			forwardMap = new Dictionary<T, int>();
			graphAlias = graphCreator();
		}

		/// <summary>
		/// Creates a graph with the same vertices and edges as a given graph.
		/// </summary>
		/// <param name="originalGraph"></param>
		/// <param name="graphCreator">Returns an instance of a graph that will be used as an underlying data structure.</param>
		public IntGraph(IGraph<T> originalGraph, Func<IGraph<int>> graphCreator)
		{
			graphAlias = graphCreator();

			ConvertGraph(originalGraph);
		}

		private void ConvertGraph(IGraph<T> originalGraph)
		{
			if (graphAlias.VerticesCount != 0)
				throw new InvalidOperationException("The graph must be empty");

			reverseMap = new List<T>(originalGraph.VerticesCount);
			forwardMap = new Dictionary<T, int>();

			foreach (var vertex in originalGraph.Vertices)
			{
				AddVertex(vertex);
			}

			foreach (var edge in originalGraph.Edges)
			{
				AddEdge(edge.From, edge.To);
			}
		}

		/// <summary>
		/// Adds a given vertex and creates a mapping from the vertex to an int.
		/// </summary>
		/// <param name="vertex"></param>
		public void AddVertex(T vertex)
		{
			graphAlias.AddVertex(reverseMap.Count);
			forwardMap.Add(vertex, reverseMap.Count);
			reverseMap.Add(vertex);
		}

		/// <summary>
		/// Adds an edge to the graph.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public void AddEdge(T from, T to)
		{
			graphAlias.AddEdge(GetVertexAlias(from), GetVertexAlias(to));
		}

		/// <summary>
		/// Gets the original vertex for a given integer alias.
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		public T GetOriginalVertex(int vertex)
		{
			if (vertex >= VerticesCount)
				throw new InvalidOperationException();

			return reverseMap[vertex];
		}

		/// <summary>
		/// Gets the alias for a given vertex.
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		public int GetVertexAlias(T vertex)
		{
			if (!forwardMap.TryGetValue(vertex, out var alias))
				throw new ArgumentException("Given vertex does not exist in the graph");

			return alias;
		}

		/// <summary>
		/// Adding integer vertices is not supported. Use AddVertex(T vertex).
		/// </summary>
		/// <param name="vertex"></param>
		void IGraph<int>.AddVertex(int vertex)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Adding integer edges is not supported. Use AddEdge(T from, T to).
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		void IGraph<int>.AddEdge(int from, int to)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public IEnumerable<int> GetNeighbours(int vertex)
		{
			return graphAlias.GetNeighbours(vertex);
		}

		/// <inheritdoc />
		public bool HasEdge(int from, int to)
		{
			return graphAlias.HasEdge(from, to);
		}
	}
}