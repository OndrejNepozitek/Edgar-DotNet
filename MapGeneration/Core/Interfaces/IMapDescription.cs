namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface IMapDescription<TNode>
	{
		IReadOnlyCollection<IRoomDescription> GetRooms();

		Graph<TNode> GetGraph();
	}
}