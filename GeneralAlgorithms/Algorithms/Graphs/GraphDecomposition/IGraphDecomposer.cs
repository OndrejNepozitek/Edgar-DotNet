namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition
{
	using System.Collections.Generic;
	using DataStructures.Graphs;

	/// <summary>
	/// Represents types that are capable to compute faces of a planar graph.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface IGraphDecomposer<TNode>
	{
		/// <summary>
		/// Gets faces of a given graph.
		/// </summary>
		/// <param name="graph">Planar graph.</param>
		/// <returns></returns>
		List<List<TNode>> GetFaces(IGraph<TNode> graph);
	}
}