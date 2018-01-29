namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition
{
	using System.Collections.Generic;
	using DataStructures.Graphs;

	public interface IGraphDecomposer<TNode>
	{
		List<List<TNode>> GetFaces(IGraph<TNode> graph);
	}
}