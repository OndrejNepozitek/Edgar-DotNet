using System.Linq;

namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A graph where edges are represented as an adjacency lists.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class UndirectedAdjacencyListGraph<T> : IGraph<T>
	{
		/// <inheritdoc />
		public bool IsDirected { get; } = false;

		/// <inheritdoc />
		public IEnumerable<T> Vertices => adjacencyLists.Keys;

		/// <inheritdoc />
		public IEnumerable<IEdge<T>> Edges => GetEdges();

		/// <inheritdoc />
		public int VerticesCount => adjacencyLists.Count;

		private readonly Dictionary<T, List<T>> adjacencyLists = new Dictionary<T, List<T>>();

		public UndirectedAdjacencyListGraph() {}

		// TODO: how to handle properly?
        public UndirectedAdjacencyListGraph(UndirectedAdjacencyListGraph<T> graph)
        {
			adjacencyLists = new Dictionary<T, List<T>>();

            foreach (var pair in graph.adjacencyLists)
            {
                adjacencyLists[pair.Key] = pair.Value.ToList();
            }
        }

		/// <inheritdoc />
		public void AddVertex(T vertex)
		{
			if (adjacencyLists.ContainsKey(vertex))
				throw new ArgumentException("Vertex already exists");

			adjacencyLists[vertex] = new List<T>();
		}

		/// <inheritdoc />
		public void AddEdge(T from, T to)
		{
			if (!adjacencyLists.TryGetValue(from, out var fromList) || !adjacencyLists.TryGetValue(to, out var toList))
				throw new ArgumentException("One of the vertices does not exist");

			if (fromList.Contains(to))
				throw new ArgumentException("The edge was already added");

			fromList.Add(to);
			toList.Add(from);
		}

		/// <inheritdoc />
		public IEnumerable<T> GetNeighbours(T vertex)
		{
			if (!adjacencyLists.TryGetValue(vertex, out var neighbours))
				throw new ArgumentException("The vertex does not exist");

			return neighbours;
		}

		/// <inheritdoc />
		public bool HasEdge(T from, T to)
		{
			foreach (var neighbour in GetNeighbours(from))
			{
				if (neighbour.Equals(to))
					return true;
			}

			return false;
		}

		private IEnumerable<IEdge<T>> GetEdges()
		{
			var usedEdges = new HashSet<Tuple<T, T>>();
			var edges = new List<IEdge<T>>();

			foreach (var pair in adjacencyLists)
			{
				var vertex = pair.Key;
				var neighbours = pair.Value;

				foreach (var neighbour in neighbours)
				{
					if (usedEdges.Contains(Tuple.Create(vertex, neighbour)) || usedEdges.Contains(Tuple.Create(neighbour, vertex)))
						continue;

					edges.Add(new Edge<T>(vertex, neighbour));
					usedEdges.Add(Tuple.Create(vertex, neighbour));
				}
			}

			return edges;
		}
	}
}