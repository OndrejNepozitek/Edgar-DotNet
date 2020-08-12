using System.Collections.Generic;
using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators
{
    public static class Helpers
    {
        public static IMapDescription<TNode> GetMapDescription<TNode>(this LevelDescriptionGrid2D<TNode> levelDescription)
        {
            var mapDescription = new MapDescription<TNode>();
            var graph = levelDescription.GetGraph();

            var corridorRoomDescriptions = new Dictionary<RoomDescriptionGrid2D, CorridorRoomDescription>();

            foreach (var room in graph.Vertices)
            {
                var roomDescription = levelDescription.GetRoomDescription(room);

                if (roomDescription.IsCorridor)
                {
                    if (corridorRoomDescriptions.TryGetValue(roomDescription, out var cached))
                    {
                        mapDescription.AddRoom(room, cached);
                    }
                    else
                    {
                        var corridorRoomDescription = new CorridorRoomDescription(roomDescription.RoomTemplates);
                        corridorRoomDescriptions[roomDescription] = corridorRoomDescription;
                        mapDescription.AddRoom(room, corridorRoomDescription);
                    }
                }
                else
                {
                    mapDescription.AddRoom(room, new BasicRoomDescription(roomDescription.RoomTemplates));
                }
            }

            foreach (var edge in graph.Edges)
            {
                mapDescription.AddConnection(edge.From, edge.To);
            }

            return mapDescription;
        }
    }
}