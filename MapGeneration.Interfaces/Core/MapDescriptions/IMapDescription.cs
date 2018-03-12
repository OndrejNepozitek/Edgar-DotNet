namespace MapGeneration.Interfaces.Core.MapDescriptions
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface IMapDescription<TNode>
	{
		IGraph<TNode> GetGraph(); 
	}
}