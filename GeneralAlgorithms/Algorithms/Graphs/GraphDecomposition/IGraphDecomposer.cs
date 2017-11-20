namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using global::DataStructures.Graphs;

	public interface IGraphDecomposer<TNode> where TNode : IComparable<TNode>
	{
		List<List<TNode>> GetChains(IGraph<TNode> graph);
	}
}