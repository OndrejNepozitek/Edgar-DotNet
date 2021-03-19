using Edgar.GraphBasedGenerator.Common.RoomTemplates;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Common
{
    /// <summary>
    /// Represents a high level description of the level.
    /// </summary>
    /// <typeparam name="TRoom"></typeparam>
    public interface ILevelDescription<TRoom>
    {
        /// <summary>
        /// Gets an undirected graph of the level.
        /// </summary>
        /// <returns></returns>
        IGraph<TRoom> GetGraph();

        /// <summary>
        /// Gets an undirected graph of the level without corridor rooms.
        /// </summary>
        /// <returns></returns>
        IGraph<TRoom> GetGraphWithoutCorridors();

        /// <summary>
        /// Gets a graph of the level.
        /// </summary>
        /// <param name="withoutCorridors"></param>
        /// <param name="directed"></param>
        /// <returns></returns>
        IGraph<TRoom> GetGraph(bool withoutCorridors, bool directed);

        /// <summary>
        /// Gets room description of a given room.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        IRoomDescription GetRoomDescription(TRoom room);
    }
}