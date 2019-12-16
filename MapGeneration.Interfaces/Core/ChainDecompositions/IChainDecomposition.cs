namespace MapGeneration.Interfaces.Core.ChainDecompositions
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	/// <summary>
	/// Represents an algorithm that can decompose graph into disjunct chains covering all vertices.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface IChainDecomposition<TNode>
	{
		IList<IChain<TNode>> GetChains(IGraph<TNode> graph);
	}
}