namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface IChainDecomposition<TNode>
	{
		List<List<TNode>> GetChains(IGraph<TNode> graph);
	}
}