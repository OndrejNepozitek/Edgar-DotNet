namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;

	public class IntGraph<T> : IGraph<int>
	{
		public IEnumerable<int> Vertices => newGraph.Vertices;

		public IEnumerable<IEdge<int>> Edges => newGraph.Edges;

		public int VerticesCount => newGraph.VerticesCount;

		private Dictionary<T, int> forwardMap;

		private List<T> reverseMap;

		private readonly IGraph<int> newGraph;

		public IntGraph(Func<IGraph<int>> graphCreator)
		{
			reverseMap = new List<T>();
			forwardMap = new Dictionary<T, int>();
			newGraph = graphCreator();
		}

		public IntGraph(IGraph<T> originalGraph, Func<IGraph<int>> graphCreator)
		{
			newGraph = graphCreator();

			ConvertGraph(originalGraph);
		}

		private void ConvertGraph(IGraph<T> originalGraph)
		{
			if (newGraph.VerticesCount != 0)
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

		public void AddVertex(T vertex)
		{
			newGraph.AddVertex(reverseMap.Count);
			forwardMap.Add(vertex, reverseMap.Count);
			reverseMap.Add(vertex);
		}

		public void AddEdge(T from, T to)
		{
			newGraph.AddEdge(GetNewVertex(from), GetNewVertex(to));
		}

		public T GetOriginalVertex(int vertex)
		{
			if (vertex >= VerticesCount)
				throw new InvalidOperationException();

			return reverseMap[vertex];
		}

		public int GetNewVertex(T vertex)
		{
			return forwardMap[vertex];
		}

		void IGraph<int>.AddVertex(int vertex)
		{
			throw new NotSupportedException();
		}

		void IGraph<int>.AddEdge(int from, int to)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<int> GetNeighbours(int vertex)
		{
			return newGraph.GetNeighbours(vertex);
		}

		public int GetNeighbourIndex(int vertex, int neighbour)
		{
			throw new System.NotImplementedException();
		}

		public bool HasEdge(int from, int to)
		{
			return newGraph.HasEdge(from, to);
		}
	}
}