namespace GeneralAlgorithms.Algorithms.Graphs
{
	using System.Collections.Generic;
	using System.Linq;
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
		/// <param name="graph"></param>
		/// <returns></returns>
		public bool IsConnected<T>(IGraph<T> graph)
		{
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
	}
}