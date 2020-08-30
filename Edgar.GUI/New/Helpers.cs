using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;

namespace Edgar.GUI.New
{
    public static class Helpers
    {
        public static LevelDescriptionGrid2D<TNode> GetLevelDescription<TNode>(this IMapDescription<TNode> mapDescription)
        {
            var levelDescription = new LevelDescriptionGrid2D<TNode>();
            var graph = mapDescription.GetGraph();

            var corridorRoomDescriptions = new Dictionary<IRoomDescription, RoomDescriptionGrid2D>();
            var roomTemplateMapping = new Dictionary<RoomTemplate, RoomTemplateGrid2D>();

            foreach (var room in graph.Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(room);

                if (roomDescription.IsCorridor)
                {
                    if (corridorRoomDescriptions.TryGetValue(roomDescription, out var cached))
                    {
                        levelDescription.AddRoom(room, cached);
                    }
                    else
                    {
                        var corridorRoomDescription = new RoomDescriptionGrid2D(true, roomDescription.RoomTemplates.Select(x => GetNewRoomTemplate(x, roomTemplateMapping)).ToList());
                        corridorRoomDescriptions[roomDescription] = corridorRoomDescription;
                        levelDescription.AddRoom(room, corridorRoomDescription);
                    }
                }
                else
                {
                    levelDescription.AddRoom(room, new RoomDescriptionGrid2D(false, roomDescription.RoomTemplates.Select(x => GetNewRoomTemplate(x, roomTemplateMapping)).ToList()));
                }
            }

            foreach (var edge in graph.Edges)
            {
                levelDescription.AddConnection(edge.From, edge.To);
            }

            return levelDescription;
        }

        private static RoomTemplateGrid2D GetNewRoomTemplate(RoomTemplate roomTemplate, Dictionary<RoomTemplate, RoomTemplateGrid2D> mapping)
        {
            if (mapping.TryGetValue(roomTemplate, out var cached))
            {
                return cached;
            }

            var oldRoomTemplate = roomTemplate.ToOldRoomTemplate();
            mapping[roomTemplate] = oldRoomTemplate;

            return oldRoomTemplate;
        }

        public static RoomTemplateGrid2D ToOldRoomTemplate(this RoomTemplate roomTemplate)
        {
            var doorMode = roomTemplate.DoorsMode;
            IDoorModeGrid2D oldDoorMode = null;

            if (roomTemplate.RoomTemplateRepeatMode == null)
            {
                throw new NotSupportedException("Null repeat mode is currently not supported");
            }

            if (doorMode is SimpleDoorMode simpleDoorMode)
            {
                oldDoorMode = new SimpleDoorModeGrid2D(simpleDoorMode.DoorLength, simpleDoorMode.CornerDistance);
            } 
            else if (doorMode is ManualDoorMode manualDoorMode)
            {
                oldDoorMode = new ManualDoorModeGrid2D(manualDoorMode.DoorPositions.Select(x => new DoorGrid2D(x.From, x.To)).ToList());
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return new RoomTemplateGrid2D(roomTemplate.Shape, oldDoorMode, allowedTransformations: roomTemplate.AllowedTransformations, repeatMode: roomTemplate.RoomTemplateRepeatMode, name: roomTemplate.Name);

        }
    }
}