using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Graphs;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;
using SandboxEvolutionRunner.Utils;
using IRoomDescription = Edgar.Legacy.Core.MapDescriptions.Interfaces.IRoomDescription;

namespace Edgar.SandboxEvolutionRunner.Benchmarks
{
    public class LevelDescriptionLoader<TRoom>
    {
        private List<RoomTemplateGrid2D> roomTemplatesSmall;
        private List<RoomTemplateGrid2D> roomTemplatesMedium;
        private readonly List<RoomTemplateGrid2D> roomTemplatesOriginal;
        private readonly RoomTemplatesSet roomTemplatesSet;
        private readonly RoomTemplateRepeatMode repeatMode;
        private readonly Func<int, TRoom> getCorridorNameFunc;
        private readonly bool enhanceRoomTemplates = false;

        private readonly List<TransformationGrid2D> transformations =
            TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

        private readonly Vector2Int scale;

        public LevelDescriptionLoader(RoomTemplatesSet roomTemplatesSet, Vector2Int scale,
            RoomTemplateRepeatMode repeatMode = RoomTemplateRepeatMode.AllowRepeat,
            Func<int, TRoom> getCorridorNameFunc = null)
        {
            this.roomTemplatesSet = roomTemplatesSet;
            this.scale = scale;
            this.repeatMode = repeatMode;
            this.getCorridorNameFunc = getCorridorNameFunc;
            // roomTemplatesOriginal = MapDescriptionUtils.GetBasicRoomTemplates(scale);
        }

        public List<LevelDescriptionGrid2D<TRoom>> GetLevelDescriptions(List<NamedGraph<TRoom>> namedGraphs,
            List<int> corridorOffsets)
        {
            var levelDescriptions = new List<LevelDescriptionGrid2D<TRoom>>();

            foreach (var namedGraph in namedGraphs)
            {
                levelDescriptions.Add(GetLevelDescription(namedGraph, corridorOffsets));
            }

            return levelDescriptions;
        }

        protected virtual LevelDescriptionGrid2D<TRoom> GetLevelDescription(NamedGraph<TRoom> namedGraph,
            List<int> corridorOffsets)
        {
            var withCorridors = corridorOffsets[0] != 0;
            var corridorRoomDescription = withCorridors
                ? GetCorridorRoomDescription(corridorOffsets, roomTemplatesSet == RoomTemplatesSet.Original ? 1 : 2)
                : null;

            var levelDescription = new LevelDescriptionGrid2D<TRoom>();
            levelDescription.MinimumRoomDistance = 0;

            var graph = namedGraph.Graph;

            foreach (var room in graph.Vertices)
            {
                var basicRoomDescription = GetBasicRoomDescription(graph, room);
                levelDescription.AddRoom(room, basicRoomDescription);
            }

            var counter = graph.VerticesCount;

            foreach (var connection in graph.Edges)
            {
                if (withCorridors)
                {
                    levelDescription.AddRoom(getCorridorNameFunc(counter), corridorRoomDescription);
                    levelDescription.AddConnection(connection.From, getCorridorNameFunc(counter));
                    levelDescription.AddConnection(getCorridorNameFunc(counter), connection.To);
                    counter++;
                }
                else
                {
                    levelDescription.AddConnection(connection.From, connection.To);
                }
            }

            // var name = MapDescriptionUtils.GetInputName(namedGraph.Name, scale, withCorridors, corridorOffsets, canTouch);
            levelDescription.Name = namedGraph.Name;

            return levelDescription;
        }

        protected virtual RoomDescriptionGrid2D GetCorridorRoomDescription(List<int> corridorOffsets, int width = 1)
        {
            var corridorRoomTemplates = MapDescriptionUtils.GetNewCorridorRoomTemplates(corridorOffsets, width);
            var corridorRoomDescription = new RoomDescriptionGrid2D
            (
                isCorridor: true,
                roomTemplates: corridorRoomTemplates
            );

            return corridorRoomDescription;
        }

        protected virtual List<RoomTemplateGrid2D> GetSmallRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplateGrid2D>();
            var doorMode = new SimpleDoorModeGrid2D(2, 1);

            roomTemplates.Add(GetSquareRoomTemplate(6, doorMode));
            roomTemplates.Add(GetSquareRoomTemplate(8, doorMode));
            roomTemplates.Add(GetRectangleRoomTemplate(6, 8, doorMode));

            return roomTemplates;
        }

        protected virtual List<RoomTemplateGrid2D> GetMediumRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplateGrid2D>();
            var doorMode = new SimpleDoorModeGrid2D(2, 2);

            roomTemplates.Add(GetSquareRoomTemplate(12, doorMode));
            roomTemplates.Add(GetSquareRoomTemplate(14, doorMode));
            roomTemplates.Add(GetRectangleRoomTemplate(10, 14, doorMode));
            roomTemplates.Add(GetRectangleRoomTemplate(12, 15, doorMode));

            return roomTemplates;
        }

        protected virtual RoomTemplateGrid2D GetSquareRoomTemplate(int width, SimpleDoorModeGrid2D doorMode)
        {
            return GetRectangleRoomTemplate(width, width, doorMode);
        }

        protected virtual RoomTemplateGrid2D GetRectangleRoomTemplate(int width, int height,
            SimpleDoorModeGrid2D doorMode)
        {
            return GetRoomTemplate(PolygonGrid2D.GetRectangle(width, height), doorMode, $"Rectangle {width}x{width}");
        }

        protected virtual RoomTemplateGrid2D GetRoomTemplate(PolygonGrid2D polygon, SimpleDoorModeGrid2D doorMode,
            string name)
        {
            var minScale = Math.Min(scale.X, scale.Y);
            doorMode = new SimpleDoorModeGrid2D(doorMode.DoorLength * minScale, doorMode.CornerDistance * minScale);

            return new RoomTemplateGrid2D(polygon.Scale(scale), doorMode, allowedTransformations: transformations,
                name: name, repeatMode: repeatMode);
        }

        protected virtual RoomDescriptionGrid2D GetBasicRoomDescription(IGraph<TRoom> graph, TRoom vertex)
        {
            if (roomTemplatesSmall == null)
            {
                roomTemplatesSmall = GetSmallRoomTemplates();
                roomTemplatesMedium = GetMediumRoomTemplates();
            }

            var roomTemplates = new List<RoomTemplateGrid2D>();

            switch (roomTemplatesSet)
            {
                case RoomTemplatesSet.Medium:
                    roomTemplates.AddRange(roomTemplatesMedium);
                    break;

                case RoomTemplatesSet.SmallAndMedium:
                    roomTemplates.AddRange(roomTemplatesSmall);
                    roomTemplates.AddRange(roomTemplatesMedium);
                    break;
                case RoomTemplatesSet.Smart:
                {
                    if (graph.GetNeighbors(vertex).Count() <= 2)
                    {
                        roomTemplates.AddRange(roomTemplatesSmall);
                    }

                    roomTemplates.AddRange(roomTemplatesMedium);
                    break;
                }
                case RoomTemplatesSet.Original:
                    roomTemplates.AddRange(roomTemplatesOriginal);
                    break;
            }

            return new RoomDescriptionGrid2D
            (
                isCorridor: false,
                roomTemplates: roomTemplates
            );
        }
    }

    public class LevelDescriptionLoader : LevelDescriptionLoader<int>
    {
        public LevelDescriptionLoader(RoomTemplatesSet roomTemplatesSet, Vector2Int scale,
            RoomTemplateRepeatMode repeatMode = RoomTemplateRepeatMode.AllowRepeat) : base(roomTemplatesSet, scale,
            repeatMode, x => x)
        {
        }
    }
}