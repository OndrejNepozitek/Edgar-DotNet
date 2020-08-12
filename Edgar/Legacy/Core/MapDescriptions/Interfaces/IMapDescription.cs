using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.Legacy.Core.MapDescriptions.Interfaces
{
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

        /// <summary>
        /// Gets the graph of stage-one rooms.
        /// </summary>
        /// <returns></returns>
        IGraph<TNode> GetStageOneGraph();

        IRoomDescription GetRoomDescription(TNode node);
    }
}