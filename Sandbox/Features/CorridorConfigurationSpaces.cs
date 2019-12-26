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
using MapGeneration.Benchmarks.ResultSaving;
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
                new GeneratorInput<MapDescription<int>>("Example 1 (fig. 1)", GetExample(GraphsDatabase.GetExample1())),
                new GeneratorInput<MapDescription<int>>("Example 2 (fig. 7 top)", GetExample(GraphsDatabase.GetExample2())),
                new GeneratorInput<MapDescription<int>>("Example 3 (fig. 7 bottom)", GetExample(GraphsDatabase.GetExample3())),
                new GeneratorInput<MapDescription<int>>("Example 4 (fig. 8)", GetExample(GraphsDatabase.GetExample4())),
                new GeneratorInput<MapDescription<int>>("Example 5 (fig. 9)", GetExample(GraphsDatabase.GetExample5())),
                new GeneratorInput<MapDescription<int>>("Example 1 (fig. 1) wc", GetExample(GraphsDatabase.GetExample1(), true)),
                new GeneratorInput<MapDescription<int>>("Example 2 (fig. 7 top) wc", GetExample(GraphsDatabase.GetExample2(), true)),
                new GeneratorInput<MapDescription<int>>("Example 3 (fig. 7 bottom) wc", GetExample(GraphsDatabase.GetExample3(), true)),
                new GeneratorInput<MapDescription<int>>("Example 4 (fig. 8) wc", GetExample(GraphsDatabase.GetExample4(), true)),
                new GeneratorInput<MapDescription<int>>("Example 5 (fig. 9) wc", GetExample(GraphsDatabase.GetExample5(), true)),
            };

            var benchmarkRunner = new BenchmarkRunner<MapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<MapDescription<int>>("CorridorConfigurationSpaces_Reference", input =>
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
            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResult(scenarioResult);

            Utils.BenchmarkUtils.IsEqualToReference(scenarioResult, "BenchmarkResults/1577179644_CorridorConfigurationSpaces_Reference.json");
        }

        private MapDescription<int> GetExample(IGraph<int> graph, bool withCorridors = false)
        {
            var mapDescription = new MapDescription<int>();

            var basicRoomTemplates = GetBasicRoomTemplates(new IntVector2(1, 1));
            var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

            var corridorRoomTemplate = new List<IRoomTemplate>()
            {
                new RoomTemplate(
                    GridPolygon.GetRectangle(1, 1),
                    new SpecificPositionsMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 1)),
                        new OrthogonalLine(new IntVector2(1, 0), new IntVector2(1, 1)),
                    }),
                    TransformationHelper.GetAllTransformations().ToList()
                )
            };
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplate);

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