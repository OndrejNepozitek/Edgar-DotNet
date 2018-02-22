namespace GeneralAlgorithms.DataStructures.Graphs
{
	/// <summary>
	/// Interface describing an edge of a graph.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEdge<T>
	{
		T From { get; }

		T To { get; }
	}
}