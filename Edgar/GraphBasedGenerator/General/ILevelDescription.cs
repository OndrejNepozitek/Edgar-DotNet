using Edgar.GraphBasedGenerator.General.RoomTemplates;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.GraphBasedGenerator.General
{
    public interface ILevelDescription<TRoom>
    {
        IGraph<TRoom> GetGraph();

        IGraph<TRoom> GetGraphWithoutCorridors();

        IRoomDescription GetRoomDescription(TRoom node);
    }
}