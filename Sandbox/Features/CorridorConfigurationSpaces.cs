using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapDescriptions;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class CorridorConfigurationSpaces
    {
        public void Run()
        {
            var inputs = new List<GeneratorInput<MapDescription<int>>>()
            {
                new GeneratorInput<MapDescription<int>>("Example 1", GetExample(GraphsDatabase.GetExample1())),
                new GeneratorInput<MapDescription<int>>("Example 2", GetExample(GraphsDatabase.GetExample2())),
                new GeneratorInput<MapDescription<int>>("Example 3", GetExample(GraphsDatabase.GetExample3())),
                new GeneratorInput<MapDescription<int>>("Example 4", GetExample(GraphsDatabase.GetExample4())),
                new GeneratorInput<MapDescription<int>>("Example 5", GetExample(GraphsDatabase.GetExample5())),
            };

            var benchmarkRunner = new BenchmarkRunner<MapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<MapDescription<int>>("CorridorConfigurationSpaces", input =>
            {
                
                var layoutGenerator = new DungeonGenerator<int>(input.MapDescription);
                layoutGenerator.InjectRandomGenerator(new Random(0));

                return new LambdaGeneratorRunner(() =>
                {
                    var layouts = layoutGenerator.GenerateLayout();

                    return new GeneratorRun(layouts != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount);
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 500);
        }

        private MapDescription<int> GetExample(IGraph<int> graph)
        {
            var mapDescription = new MapDescription<int>();

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