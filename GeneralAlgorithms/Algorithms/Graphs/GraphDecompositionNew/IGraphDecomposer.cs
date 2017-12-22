namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecompositionNew
{
	using System.Collections.Generic;
	using DataStructures.Graphs;

	public interface IGraphDecomposer<TNode>
	{
		List<List<TNode>> GetChains(FastGraph<TNode> graph);
	}
}