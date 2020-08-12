using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;

namespace SandboxEvolutionRunner.Utils
{
    public class BetterMapDescriptionLoader : MapDescriptionLoader
    {
        private readonly List<RoomTemplate> roomTemplatesSmall;
        private readonly List<RoomTemplate> roomTemplatesMedium;
        private readonly List<RoomTemplate> roomTemplatesOriginal;
        private readonly RoomTemplatesSet roomTemplatesSet;
        private readonly RepeatMode repeatMode;
        private readonly bool enhanceRoomTemplates = false;
        private readonly List<Transformation> transformations = TransformationHelper.GetAllTransformations().ToList();

        public BetterMapDescriptionLoader(Options options, RoomTemplatesSet roomTemplatesSet, RepeatMode repeatMode = RepeatMode.AllowRepeat) : base(options)
        {
            this.roomTemplatesSet = roomTemplatesSet;
            this.repeatMode = repeatMode;
            roomTemplatesSmall = GetSmallRoomTemplates();
            roomTemplatesMedium = GetMediumRoomTemplates();
            roomTemplatesOriginal = MapDescriptionUtils.GetBasicRoomTemplates(Options.Scale);
        }

        private List<RoomTemplate> GetSmallRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplate>();
            var doorMode = new SimpleDoorMode(2, 1);

            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetSquare(6), doorMode, transformations, name: "Square 6x6", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetSquare(8), doorMode, transformations, name: "Square 8x8", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetRectangle(6, 8), doorMode, transformations, name: "Rectangle 6x8", repeatMode: repeatMode));

            if (enhanceRoomTemplates)
            {
                roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetSquare(7), doorMode, transformations, name: "Square 7x7", repeatMode: repeatMode));
                roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetRectangle(5, 7), doorMode, transformations, name: "Rectangle 5x7", repeatMode: repeatMode));
            }

            return roomTemplates;
        }

        private List<RoomTemplate> GetMediumRoomTemplates()
        {
            var roomTemplates = new List<RoomTemplate>();
            var doorMode = new SimpleDoorMode(2, 2);

            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetSquare(12), doorMode, transformations, name: "Square 12x12", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetSquare(14), doorMode, transformations, name: "Square 14x14", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetRectangle(10, 14), doorMode, transformations, name: "Rectangle 10x14", repeatMode: repeatMode));
            roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetRectangle(12, 15), doorMode, transformations, name: "Rectangle 12x15", repeatMode: repeatMode));

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
                roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetSquare(13), doorMode, transformations, name: "Square 13x13", repeatMode: repeatMode));
                roomTemplates.Add(new RoomTemplate(PolygonGrid2D.GetRectangle(10, 16), doorMode, transformations, name: "Rectangle 10x16", repeatMode: repeatMode));
            }

            return roomTemplates;
        }

        protected override List<NamedMapDescription> GetMapDescriptions(NamedGraph namedGraph, List<int> corridorOffsets)
        {
            var withCorridors = corridorOffsets[0] != 0;
            var canTouch = Options.CanTouch || !withCorridors;
            var corridorRoomDescription = withCorridors ? GetCorridorRoomDescription(corridorOffsets, roomTemplatesSet == RoomTemplatesSet.Original ? 1 : 2) : null;

            var mapDescription = new MapDescription<int>();
            var graph = namedGraph.Graph;

            foreach (var room in graph.Vertices)
            {
                var basicRoomDescription = GetBasicRoomDescription(graph, room);
                mapDescription.AddRoom(room, basicRoomDescription);
            }

            var counter = graph.VerticesCount;

            foreach (var connection in graph.Edges)
            {
                if (withCorridors)
                {
                    mapDescription.AddRoom(counter, corridorRoomDescription);
                    mapDescription.AddConnection(connection.From, counter);
                    mapDescription.AddConnection(connection.To, counter);
                    counter++;
                }
                else
                {
                    mapDescription.AddConnection(connection.From, connection.To);
                }
            }
            
            var name = MapDescriptionUtils.GetInputName(namedGraph.Name, Options.Scale, withCorridors, corridorOffsets, canTouch);

            return new List<NamedMapDescription>()
            {
                new NamedMapDescription(mapDescription, name, withCorridors)
            };
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