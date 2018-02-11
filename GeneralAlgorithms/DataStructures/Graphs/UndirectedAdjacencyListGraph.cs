namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;

	public class UndirectedAdjacencyListGraph<T> : IGraph<T>
	{
		public IEnumerable<T> Vertices => adjacencyLists.Keys;

		public IEnumerable<IEdge<T>> Edges => GetEdges();

		public int VerticesCount => adjacencyLists.Count;

		private readonly Dictionary<T, List<T>> adjacencyLists = new Dictionary<T, List<T>>();

		public void AddVertex(T vertex)
		{
			if (adjacencyLists.ContainsKey(vertex))
				throw new InvalidOperationException("Vertex already exists");

			adjacencyLists[vertex] = new List<T>();
		}

		public void AddEdge(T from, T to)
		{
			if (!adjacencyLists.TryGetValue(from, out var fromList) || !adjacencyLists.TryGetValue(to, out var toList))
				throw new InvalidOperationException("One of the vertices does not exist");

			if (fromList.Contains(to))
				throw new InvalidOperationException("The edge was already added");

			fromList.Add(to);
			toList.Add(from);
		}

		public IEnumerable<T> GetNeighbours(T vertex)
		{
			if (!adjacencyLists.TryGetValue(vertex, out var neighbours))
				throw new InvalidOperationException("The vertex does not exist");

			return neighbours;
		}

		public int GetNeighbourIndex(T vertex, T neighbour)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<IEdge<T>> GetEdges()
		{
			throw new System.NotImplementedException();
		}
	}
}