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
	}
}