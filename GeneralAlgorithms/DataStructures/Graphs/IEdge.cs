namespace GeneralAlgorithms.DataStructures.Graphs
{
	public interface IEdge<out T>
	{
		T From { get; }

		T To { get; }
	}
}