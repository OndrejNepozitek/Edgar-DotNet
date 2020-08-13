using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators
{
    public static class Helpers
    {
        public static IMapDescription<TNode> GetMapDescription<TNode>(this LevelDescriptionGrid2D<TNode> levelDescription)
        {
            var mapDescription = new MapDescription<TNode>();
            var graph = levelDescription.GetGraph();

            var corridorRoomDescriptions = new Dictionary<RoomDescriptionGrid2D, CorridorRoomDescription>();
            var roomTemplateMapping = new Dictionary<RoomTemplateGrid2D, RoomTemplate>();

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
                        var corridorRoomDescription = new CorridorRoomDescription(roomDescription.RoomTemplates.Select(x => GetOldRoomTemplate(x, roomTemplateMapping)).ToList());
                        corridorRoomDescriptions[roomDescription] = corridorRoomDescription;
                        mapDescription.AddRoom(room, corridorRoomDescription);
                    }
                }
                else
                {
                    mapDescription.AddRoom(room, new BasicRoomDescription(roomDescription.RoomTemplates.Select(x => GetOldRoomTemplate(x, roomTemplateMapping)).ToList()));
                }
            }

            foreach (var edge in graph.Edges)
            {
                mapDescription.AddConnection(edge.From, edge.To);
            }

            return mapDescription;
        }

        private static RoomTemplate GetOldRoomTemplate(RoomTemplateGrid2D roomTemplate, Dictionary<RoomTemplateGrid2D, RoomTemplate> mapping)
        {
            if (mapping.TryGetValue(roomTemplate, out var cached))
            {
                return cached;
            }

            var oldRoomTemplate = roomTemplate.ToOldRoomTemplate();
            mapping[roomTemplate] = oldRoomTemplate;

            return oldRoomTemplate;
        }

        public static RoomTemplate ToOldRoomTemplate(this RoomTemplateGrid2D roomTemplate)
        {
            var doorMode = roomTemplate.Doors;
            IDoorMode oldDoorMode = null;

            if (roomTemplate.RepeatMode == null)
            {
                throw new NotSupportedException("Null repeat mode is currently not supported");
            }

            if (doorMode is SimpleDoorModeGrid2D simpleDoorMode)
            {
                if (simpleDoorMode.DoorSocket != null)
                {
                    throw new NotSupportedException("Old room templates support only null sockets");
                }

                oldDoorMode = new SimpleDoorMode(simpleDoorMode.DoorLength, simpleDoorMode.CornerDistance);
            } 
            else if (doorMode is ManualDoorModeGrid2D manualDoorMode)
            {
                if (manualDoorMode.Doors.Any(x => x.Socket != null))
                {
                    throw new NotSupportedException("Old room templates support only null sockets");
                }

                oldDoorMode = new ManualDoorMode(manualDoorMode.Doors.Select(x => new OrthogonalLine(x.From, x.To)).ToList());
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return new RoomTemplate(roomTemplate.Outline, oldDoorMode, roomTemplate.AllowedTransformations, roomTemplate.RepeatMode.Value, roomTemplate.Name);

        }
    }
}