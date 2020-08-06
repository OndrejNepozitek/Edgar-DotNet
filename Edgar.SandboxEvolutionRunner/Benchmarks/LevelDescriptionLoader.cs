using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks
{
    public class LevelDescriptionLoader
    {
        private List<RoomTemplate> roomTemplatesSmall;
        private List<RoomTemplate> roomTemplatesMedium;
        private readonly List<RoomTemplate> roomTemplatesOriginal;
        private readonly RoomTemplatesSet roomTemplatesSet;
        private readonly RepeatMode repeatMode;
        private readonly bool enhanceRoomTemplates = false;
        private readonly List<Transformation> transformations = TransformationHelper.GetAllTransformations().ToList();
        private readonly IntVector2 scale;

        public LevelDescriptionLoader(RoomTemplatesSet roomTemplatesSet, IntVector2 scale, RepeatMode repeatMode = RepeatMode.AllowRepeat)
        {
            this.roomTemplatesSet = roomTemplatesSet;
            this.scale = scale;
            this.repeatMode = repeatMode;
            roomTemplatesOriginal = MapDescriptionUtils.GetBasicRoomTemplates(scale);
        }

        public List<LevelDescriptionGrid2D<int>> GetLevelDescriptions(List<NamedGraph<int>> namedGraphs, List<int> corridorOffsets)
        {
            var levelDescriptions = new List<LevelDescriptionGrid2D<int>>();

            foreach (var namedGraph in namedGraphs)
            {
                levelDescriptions.Add(GetLevelDescription(namedGraph, corridorOffsets));
            }

            return levelDescriptions;
        }

        protected virtual LevelDescriptionGrid2D<int> GetLevelDescription(NamedGraph<int> namedGraph, List<int> corridorOffsets)
        {
            var withCorridors = corridorOffsets[0] != 0;
            var corridorRoomDescription = withCorridors ? GetCorridorRoomDescription(corridorOffsets, roomTemplatesSet == RoomTemplatesSet.Original ? 1 : 2) : null;

            var levelDescription = new LevelDescriptionGrid2D<int>();
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
                    levelDescription.AddRoom(counter, corridorRoomDescription);
                    levelDescription.AddConnection(connection.From, counter);
                    levelDescription.AddConnection(connection.To, counter);
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

        protected virtual IRoomDescription GetCorridorRoomDescription(List<int> corridorOffsets, int width = 1)
        {
            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets, width);
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

            return corridorRoomDescription;
        }

        protected virtual List<RoomTemplate> GetSmallRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplate>();
            var doorMode = new SimpleDoorMode(2, 1);

            roomTemplates.Add(GetSquareRoomTemplate(6, doorMode));
            roomTemplates.Add(GetSquareRoomTemplate(8, doorMode));
            roomTemplates.Add(GetRectangleRoomTemplate(6, 8, doorMode));

            return roomTemplates;
        }

        protected virtual List<RoomTemplate> GetMediumRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplate>();
            var doorMode = new SimpleDoorMode(2, 2);

            roomTemplates.Add(GetSquareRoomTemplate(12, doorMode));
            roomTemplates.Add(GetSquareRoomTemplate(14, doorMode));
            roomTemplates.Add(GetRectangleRoomTemplate(10, 14, doorMode));
            roomTemplates.Add(GetRectangleRoomTemplate(12, 15, doorMode));

            return roomTemplates;
        }

        protected virtual RoomTemplate GetSquareRoomTemplate(int width, SimpleDoorMode doorMode)
        {
            return GetRectangleRoomTemplate(width, width, doorMode);
        }

        protected virtual RoomTemplate GetRectangleRoomTemplate(int width, int height, SimpleDoorMode doorMode)
        {
            return GetRoomTemplate(GridPolygon.GetRectangle(width, height), doorMode, $"Rectangle {width}x{width}");
        }

        protected virtual RoomTemplate GetRoomTemplate(GridPolygon polygon, SimpleDoorMode doorMode, string name)
        {
            var minScale = Math.Min(scale.X, scale.Y);
            doorMode = new SimpleDoorMode(doorMode.DoorLength * minScale, doorMode.CornerDistance * minScale);

            return new RoomTemplate(polygon.Scale(scale), doorMode, transformations, name: name, repeatMode: repeatMode);
        }

        protected virtual IRoomDescription GetBasicRoomDescription(IGraph<int> graph, int vertex)
        {
            if (roomTemplatesSmall == null)
            {
                roomTemplatesSmall = GetSmallRoomTemplates();
                roomTemplatesMedium = GetMediumRoomTemplates();
            }

            var roomTemplates = new List<RoomTemplate>();

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
                    if (graph.GetNeighbours(vertex).Count() <= 2)
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

            return new BasicRoomDescription(roomTemplates);
        }
    }
}