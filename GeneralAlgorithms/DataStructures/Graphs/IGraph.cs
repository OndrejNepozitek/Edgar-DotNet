namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System.Collections.Generic;

	public interface IGraph<T>
	{
		IEnumerable<T> Vertices { get; }

		IEnumerable<IEdge<T>> Edges { get; }

		int VerticesCount { get; }

		void AddVertex(T vertex);

		void AddEdge(T from, T to);

		IEnumerable<T> GetNeighbours(T vertex);

		/// <summary>
		/// The position of a given neighbour in the list of neighbours returned from GetNeighbours(vertex).
		/// TODO: should it be here? or make it O(N) by enumerating the neighbours?
		/// </summary>
		/// <param name="vertex"></param>
		/// <param name="neighbour"></param>
		/// <returns></returns>
		int GetNeighbourIndex(T vertex, T neighbour);
	}
}