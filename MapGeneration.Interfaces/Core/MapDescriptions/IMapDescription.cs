namespace MapGeneration.Interfaces.Core.MapDescriptions
{
	using GeneralAlgorithms.DataStructures.Graphs;

	/// <summary>
	/// Represents a description of a map to be generated.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface IMapDescription<TNode>
	{
		/// <summary>
		/// Gets the graph of rooms.
		/// </summary>
		/// <returns></returns>
		IGraph<TNode> GetGraph(); 
	}
}