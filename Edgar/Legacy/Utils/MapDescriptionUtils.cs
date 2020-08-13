using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Utils
{
    public static class MapDescriptionUtils
    {
        public static List<RoomTemplate> GetRectangularRoomTemplates(Vector2Int scale)
        {
            var overlapScale = Math.Min(scale.X, scale.Y);
            var doorMode = new SimpleDoorMode(1 * overlapScale, 0);
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            var squareRoom = new RoomTemplate(PolygonGrid2D.GetSquare(6).Scale(scale), doorMode, transformations, name: "Square");
            var rectangleRoom = new RoomTemplate(PolygonGrid2D.GetRectangle(6, 9).Scale(scale), doorMode, transformations, name: "Rectangle");

            return new List<RoomTemplate>()
            {
                squareRoom,
                rectangleRoom,
            };
        }

        public static List<RoomTemplate> GetBasicRoomTemplates(Vector2Int scale)
        {
            var overlapScale = Math.Min(scale.X, scale.Y);
            var doorMode = new SimpleDoorMode(1 * overlapScale, 0);
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            var room1 = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 6)
                    .AddPoint(3, 6)
                    .AddPoint(3, 3)
                    .AddPoint(6, 3)
                    .AddPoint(6, 0)
                    .Build().Scale(scale)
                , doorMode, transformations, name: "L-shape");
            var room2 = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 9)
                    .AddPoint(3, 9)
                    .AddPoint(3, 3)
                    .AddPoint(6, 3)
                    .AddPoint(6, 0)
                    .Build().Scale(scale)
                , doorMode, transformations, name: "L-shape long");
            var room3 = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 3)
                    .AddPoint(3, 3)
                    .AddPoint(3, 6)
                    .AddPoint(6, 6)
                    .AddPoint(6, 3)
                    .AddPoint(9, 3)
                    .AddPoint(9, 0)
                    .Build().Scale(scale)
                , doorMode, transformations, name: "T-shape");

            return new List<RoomTemplate>(GetRectangularRoomTemplates(scale))
            {
                room1,
                room2,
                room3,
            };
        }

        public static List<RoomTemplate> GetCorridorRoomTemplates(List<int> offsets, int width = 1)
        {
            if (offsets == null)
            {
                return null;
            }

            var roomTemplates = new List<RoomTemplate>();
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            foreach (var offset in offsets)
            {
                var length = offset;
                var roomTemplate = new RoomTemplate(
                    PolygonGrid2D.GetRectangle(length, width),
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(0, width)),
                        new OrthogonalLine(new Vector2Int(length, 0), new Vector2Int(length, width)),
                    }),
                    transformations
                );

                roomTemplates.Add(roomTemplate);
            }

            return roomTemplates;
        }

        public static List<RoomTemplateGrid2D> GetNewCorridorRoomTemplates(List<int> offsets, int width = 1)
        {
            if (offsets == null)
            {
                return null;
            }

            var roomTemplates = new List<RoomTemplateGrid2D>();
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            foreach (var offset in offsets)
            {
                var length = offset;
                var roomTemplate = new RoomTemplateGrid2D(
                    PolygonGrid2D.GetRectangle(length, width),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                    {
                        new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, width)),
                        new DoorGrid2D(new Vector2Int(length, 0), new Vector2Int(length, width)),
                    }),
                    allowedTransformations: transformations,
                    repeatMode: RepeatMode.AllowRepeat
                );

                roomTemplates.Add(roomTemplate);
            }

            return roomTemplates;
        }

        public static MapDescription<int> GetBasicMapDescription(IGraph<int> graph,
            IRoomDescription basicRoomDescription, IRoomDescription corridorRoomDescription = null,
            bool withCorridors = false)
        {
            var mapDescription = new MapDescription<int>();

            foreach (var room in graph.Vertices)
            {
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

            return mapDescription;
        }

        public static string GetInputName(string name, Vector2Int scale, bool withCorridors, List<int> offsets, bool canTouch, string suffix = null)
        {
            var inputName = name;

            if (scale != new Vector2Int(1, 1))
            {
                inputName += $" scale ({scale.X},{scale.Y})";
            }

            if (withCorridors)
            {
                inputName += $" wc ({string.Join(",", offsets)})";
            }

            if (!canTouch)
            {
                inputName += $" nt";
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                inputName += $" {suffix}";
            }

            return inputName;
        }
    }
}