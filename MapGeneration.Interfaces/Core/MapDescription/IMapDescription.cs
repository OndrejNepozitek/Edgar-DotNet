namespace MapGeneration.Interfaces.Core.MapDescription
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface IMapDescription<TNode>
	{
		IGraph<TNode> GetGraph(); 
	}
}