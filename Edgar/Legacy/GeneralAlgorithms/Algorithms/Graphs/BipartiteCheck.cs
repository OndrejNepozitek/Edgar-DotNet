using System;
using System.Collections.Generic;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Graphs
{
    /// <summary>
	/// Class that checks whether a graph is bipartite.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BipartiteCheck<T>
	{
		/// <summary>
		/// Checks if the graph is bipartite. If so, returns its two parts.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="parts"></param>
		/// <returns></returns>
		public bool IsBipartite(IGraph<T> graph, out Tuple<List<T>, List<T>> parts)
		{
			var coloring = new Dictionary<T, int>();

			// The coloring must be run of every component
			foreach (var vertex in graph.Vertices)
			{
				if (coloring.ContainsKey(vertex))
					continue;

				if (!DFSColor(vertex, graph, coloring))
				{
					parts = null;
					return false;
				}
			}

			// Reconstruct the parts
			var left = new List<T>();
			var right = new List<T>();

			foreach (var pair in coloring)
			{
				if (pair.Value == 0)
				{
					left.Add(pair.Key);
				}
				else
				{
					right.Add(pair.Key);
				}
			}

			parts = Tuple.Create(left, right);
			return true;
		}

		/// <summary>
		/// Greedily colors the graph with 2 colors using DFS. If a neighbour cannot be colored, the graph is not bipartite.
		/// </summary>
		/// <param name="sourceVertex"></param>
		/// <param name="graph"></param>
		/// <param name="coloring"></param>
		/// <returns></returns>
		private bool DFSColor(T sourceVertex, IGraph<T> graph, Dictionary<T, int> coloring)
		{
			var queue = new Queue<T>();
			queue.Enqueue(sourceVertex);

			coloring[sourceVertex] = 0;

			while (queue.Count != 0)
			{
				var vertex = queue.Dequeue();
				var oppositeColor = coloring[vertex] == 0 ? 1 : 0;

				foreach (var neighbour in graph.GetNeighbours(vertex))
				{
					if (coloring.TryGetValue(neighbour, out var color))
					{
						if (color != oppositeColor)
							return false;
					}
					else
					{
						coloring[neighbour] = oppositeColor;
						queue.Enqueue(neighbour);
					}
				}
			}

			return true;
		}
	}
}