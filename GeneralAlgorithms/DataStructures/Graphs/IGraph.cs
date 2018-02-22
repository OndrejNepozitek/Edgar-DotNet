namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System.Collections.Generic;

	/// <summary>
	/// Interface describing a generic graph.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IGraph<T>
	{
		/// <summary>
		/// Checks if the graph is directed.
		/// </summary>
		bool IsDirected { get; }

		/// <summary>
		/// Gets all vertices of the graph.
		/// </summary>
		IEnumerable<T> Vertices { get; }

		/// <summary>
		/// Gets all edges of the graph.
		/// </summary>
		IEnumerable<IEdge<T>> Edges { get; }

		/// <summary>
		/// Gets the total number of vertices.
		/// </summary>
		int VerticesCount { get; }

		/// <summary>
		/// Adds a vertex.
		/// </summary>
		/// <param name="vertex"></param>
		void AddVertex(T vertex);

		/// <summary>
		/// Adds an edge.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		void AddEdge(T from, T to);

		/// <summary>
		/// Gets all neighbours of a given vertex.
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		IEnumerable<T> GetNeighbours(T vertex);

		/// <summary>
		/// The position of a given neighbour in the list of neighbours returned from GetNeighbours(vertex).
		/// TODO: should it be here? or make it O(N) by enumerating the neighbours?
		/// </summary>
		/// <param name="vertex"></param>
		/// <param name="neighbour"></param>
		/// <returns></returns>
		int GetNeighbourIndex(T vertex, T neighbour);

		/// <summary>
		/// Checks whether there exists a given edge.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		bool HasEdge(T from, T to);
	}
}