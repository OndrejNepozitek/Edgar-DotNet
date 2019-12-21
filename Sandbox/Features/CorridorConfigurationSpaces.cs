using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Benchmarks;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapDescriptions;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class CorridorConfigurationSpaces
    {
        public void Run()
        {
            //var dummyInput = new GeneratorInput<MapDescription<int>>("Example 4", GetExample4());
            //var benchmarkRunner = new BenchmarkRunner<MapDescription<int>>();
            //var benchmarkScenario = new BenchmarkScenario<MapDescription<int>>("CorridorConfigurationSpaces", input =>
            //{

            //});
        }

        private MapDescription<int> GetExample4()
        {
            var mapDescription = new MapDescription<int>();

            var graph = GraphsDatabase.GetExample4();
            var basicRoomTemplates = GetBasicRoomTemplates(new IntVector2(1, 1));
            var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

            foreach (var room in graph.Vertices)
            {
                mapDescription.AddRoom(room, basicRoomDescription);
            }

            foreach (var connection in graph.Edges)
            {
                mapDescription.AddConnection(connection.From, connection.To);
            }

            return mapDescription;
        }

        private List<IRoomTemplate> GetBasicRoomTemplates(IntVector2 scale)
        {
            var overlapScale = Math.Min(scale.X, scale.Y);
            var doorMode = new OverlapMode(1 * overlapScale, 0);
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
	}
}