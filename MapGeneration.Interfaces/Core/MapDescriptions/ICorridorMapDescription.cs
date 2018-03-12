namespace MapGeneration.Interfaces.Core.MapDescriptions
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ICorridorMapDescription<TNode> : IMapDescription<TNode>
	{
		bool IsWithCorridors { get; }

		List<int> CorridorsOffsets { get; }

		bool IsCorridorRoom(TNode room);

		IGraph<TNode> GetGraphWithoutCorrridors();
	}
}