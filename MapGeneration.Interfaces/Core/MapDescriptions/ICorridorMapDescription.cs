namespace MapGeneration.Interfaces.Core.MapDescriptions
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	/// <summary>
	/// Represents a map description with a support of corridors.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface ICorridorMapDescription<TNode> : IMapDescription<TNode>
	{
		/// <summary>
		/// Checks if corridors are enabled.
		/// </summary>
		bool IsWithCorridors { get; }

		/// <summary>
		/// Gets offsets of rooms when using corridors.
		/// </summary>
		List<int> CorridorsOffsets { get; }

		/// <summary>
		/// Checks if a given room is a corridor room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		bool IsCorridorRoom(TNode room);

		/// <summary>
		/// Gets the original graph without corridors.
		/// </summary>
		/// <returns></returns>
		IGraph<TNode> GetGraphWithoutCorrridors();
	}
}