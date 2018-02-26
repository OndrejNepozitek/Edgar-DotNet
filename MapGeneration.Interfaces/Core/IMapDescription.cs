namespace MapGeneration.Interfaces.Core
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface IMapDescription<TNode>
	{
		// TODO: how should this look like?
		FastGraph<TNode> GetGraph(); 
	}
}