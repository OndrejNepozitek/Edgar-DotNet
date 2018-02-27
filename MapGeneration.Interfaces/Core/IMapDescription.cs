namespace MapGeneration.Interfaces.Core
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface IMapDescription<TNode>
	{
		IGraph<TNode> GetGraph(); 
	}
}