namespace MapGeneration.Interfaces.Core.MapDescription
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ICorridorMapDescription<TNode> : IMapDescription<TNode>
	{
		bool IsWithCorridors { get; }

		bool IsCorridorRoom(TNode room);

		IGraph<TNode> GetGraphWithoutCorrridors();
	}
}