using Edgar.GraphBasedGenerator.RoomTemplates;
using GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.GraphBasedGenerator
{
    public interface ILevelDescription<TRoom>
    {
        IGraph<TRoom> GetGraph();

        IGraph<TRoom> GetGraphWithoutCorridors();

        IRoomDescription GetRoomDescription(TRoom node);
    }
}