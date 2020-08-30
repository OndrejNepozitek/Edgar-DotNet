using Edgar.GraphBasedGenerator.Common.RoomTemplates;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Common
{
    public interface ILevelDescription<TRoom>
    {
        IGraph<TRoom> GetGraph();

        IGraph<TRoom> GetGraphWithoutCorridors();

        IRoomDescription GetRoomDescription(TRoom node);
    }
}