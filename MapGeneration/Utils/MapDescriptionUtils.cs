using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapDescriptions;

namespace MapGeneration.Utils
{
    public static class MapDescriptionUtils
    {
        public static List<IRoomTemplate> GetBasicRoomTemplates(IntVector2 scale)
        {
            var overlapScale = Math.Min(scale.X, scale.Y);
            var doorMode = new SimpleDoorMode(1 * overlapScale, 0);
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            var squareRoom = new RoomTemplate(GridPolygon.GetSquare(6).Scale(scale), doorMode, transformations);
            var rectangleRoom = new RoomTemplate(GridPolygon.GetRectangle(6, 9).Scale(scale), doorMode, transformations);
            var room1 = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 6)
                    .AddPoint(3, 6)
                    .AddPoint(3, 3)
                    .AddPoint(6, 3)
                    .AddPoint(6, 0)
                    .Build().Scale(scale)
                , doorMode, transformations);
            var room2 = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 9)
                    .AddPoint(3, 9)
                    .AddPoint(3, 3)
                    .AddPoint(6, 3)
                    .AddPoint(6, 0)
                    .Build().Scale(scale)
                , doorMode, transformations);
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
                , doorMode, transformations);

            return new List<IRoomTemplate>()
            {
                squareRoom,
                rectangleRoom,
                room1,
                room2,
                room3,
            };
        }

        public static List<IRoomTemplate> GetCorridorRoomTemplates(List<int> offsets)
        {
            if (offsets == null)
            {
                return null;
            }

            var roomTemplates = new List<IRoomTemplate>();
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            foreach (var offset in offsets)
            {
                var width = offset;
                var roomTemplate = new RoomTemplate(
                    GridPolygon.GetRectangle(width, 1),
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 1)),
                        new OrthogonalLine(new IntVector2(width, 0), new IntVector2(width, 1)),
                    }),
                    transformations
                );

                roomTemplates.Add(roomTemplate);
            }

            return roomTemplates;
        }

        public static MapDescription<int> GetBasicMapDescription(IGraph<int> graph,
            BasicRoomDescription basicRoomDescription, CorridorRoomDescription corridorRoomDescription = null,
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

        public static string GetInputName(string name, IntVector2 scale, bool withCorridors, List<int> offsets, bool canTouch)
        {
            var inputName = name;

            if (scale != new IntVector2(1, 1))
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

            return inputName;
        }
    }
}