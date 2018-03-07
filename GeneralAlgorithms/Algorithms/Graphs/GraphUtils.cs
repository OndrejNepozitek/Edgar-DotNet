namespace GeneralAlgorithms.Algorithms.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using DataStructures.Graphs;

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

				foreach (var neighbour in graph.GetNeighbours(vertex))
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

			var alias = new IntGraph<T>(graph, () => new UndirectedAdjacencyListGraph<int>());

			// Prepare edges for the C++ code
			var edges = new List<int>();

			foreach (var edge in alias.Edges)
			{
				edges.Add(edge.From);
				edges.Add(edge.To);
			}

			return BoostWrapper.IsPlanar(edges.ToArray(), edges.Count, alias.VerticesCount);
		}
	}
}