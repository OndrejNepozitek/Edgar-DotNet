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
        private readonly List<RoomTemplate> roomTemplatesSmall;
        private readonly List<RoomTemplate> roomTemplatesMedium;
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
            roomTemplatesSmall = GetSmallRoomTemplates();
            roomTemplatesMedium = GetMediumRoomTemplates();
            roomTemplatesOriginal = MapDescriptionUtils.GetBasicRoomTemplates(scale);
        }

        public List<GraphBasedLevelDescription<int>> GetLevelDescriptions(List<NamedGraph<int>> namedGraphs, List<int> corridorOffsets)
        {
            var levelDescriptions = new List<GraphBasedLevelDescription<int>>();

            foreach (var namedGraph in namedGraphs)
            {
                levelDescriptions.Add(GetLevelDescription(namedGraph, corridorOffsets));
            }

            return levelDescriptions;
        }

        protected GraphBasedLevelDescription<int> GetLevelDescription(NamedGraph<int> namedGraph, List<int> corridorOffsets)
        {
            var withCorridors = corridorOffsets[0] != 0;
            var corridorRoomDescription = withCorridors ? GetCorridorRoomDescription(corridorOffsets, roomTemplatesSet == RoomTemplatesSet.Original ? 1 : 2) : null;

            var levelDescription = new GraphBasedLevelDescription<int>();
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

                private List<RoomTemplate> GetSmallRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplate>();
            var doorMode = new SimpleDoorMode(2, 1);

            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(6), doorMode, transformations, name: "Square 6x6", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(8), doorMode, transformations, name: "Square 8x8", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(6, 8), doorMode, transformations, name: "Rectangle 6x8", repeatMode: repeatMode));

            if (enhanceRoomTemplates)
            {
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(7), doorMode, transformations, name: "Square 7x7", repeatMode: repeatMode));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(5, 7), doorMode, transformations, name: "Rectangle 5x7", repeatMode: repeatMode));
            }

            return roomTemplates;
        }

        private List<RoomTemplate> GetMediumRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplate>();
            var doorMode = new SimpleDoorMode(2, 2);

            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(12), doorMode, transformations, name: "Square 12x12", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(14), doorMode, transformations, name: "Square 14x14", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(10, 14), doorMode, transformations, name: "Rectangle 10x14", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(12, 15), doorMode, transformations, name: "Rectangle 12x15", repeatMode: repeatMode));

            //roomTemplates.Add(new RoomTemplate(
            //    new GridPolygonBuilder()
            //        .AddPoint(0, 0)
            //        .AddPoint(0, 16)
            //        .AddPoint(8, 16)
            //        .AddPoint(8, 8)
            //        .AddPoint(16, 8)
            //        .AddPoint(16, 0)
            //        .Build()
            //    , doorMode, transformations, name: "L-shape large", repeatMode: RepeatMode.NoRepeat));

            if (enhanceRoomTemplates)
            {
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(13), doorMode, transformations, name: "Square 13x13", repeatMode: repeatMode));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(10, 16), doorMode, transformations, name: "Rectangle 10x16", repeatMode: repeatMode));
            }

            return roomTemplates;
        }

        private IRoomDescription GetBasicRoomDescription(IGraph<int> graph, int vertex)
        {
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