namespace GeneralAlgorithms.DataStructures.Graphs
{
	public class Edge<T> : IEdge<T>
	{
		public T From { get; }

		public T To { get; }

		public Edge(T from, T to)
		{
			From = from;
			To = to;
		}
	}
}