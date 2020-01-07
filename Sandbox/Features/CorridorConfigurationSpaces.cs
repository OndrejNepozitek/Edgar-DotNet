using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using GUI;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Stats;
using MapGeneration.MetaOptimization.Visualizations;
using MapGeneration.Utils;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class CorridorConfigurationSpaces
    {
        private void ShowVisualization()
        {
            var input = GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() {2, 4, 6}, false)[4];

            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, input.Configuration, input.Offsets);
            layoutGenerator.InjectRandomGenerator(new Random(0));

            var settings = new GeneratorSettings
            {
                LayoutGenerator = layoutGenerator,

                NumberOfLayouts = 10,

                ShowPartialValidLayouts = false,
                ShowPartialValidLayoutsTime = 500,

                ShowPerturbedLayouts = true,
                ShowPerturbedLayoutsTime = 1000,

                ShowFinalLayouts = true,
            };

            Application.Run(new GeneratorWindow(settings));
        }

        public void Run()
        {
            //ShowVisualization();
            //return;

            var inputs = new List<DungeonGeneratorInput<int>>();
            inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), false, null, true));
            inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2 }, true));
            inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4, 6, 8 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4, 6 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2 }, false));

            if (true)
            {
                inputs.Sort((x1, x2) => string.Compare(x1.Name, x2.Name, StringComparison.Ordinal));
            }

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("CorridorConfigurationSpaces", input =>
            {
                var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
                var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration, dungeonGeneratorInput.Offsets);
                layoutGenerator.InjectRandomGenerator(new Random(0));

                //return new LambdaGeneratorRunner(() =>
                //{
                //    var layouts = layoutGenerator.GenerateLayout();

                //    return new GeneratorRun(layouts != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount);
                //});
                return new LambdaGeneratorRunner(() =>
                {
                    var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();
                    void SimulatedAnnealingEventHandler(object sender, SimulatedAnnealingEventArgs eventArgs)
                    {
                        simulatedAnnealingArgsContainer.Add(eventArgs);
                    }

                    layoutGenerator.OnSimulatedAnnealingEvent += SimulatedAnnealingEventHandler;
                    var layout = layoutGenerator.GenerateLayout();
                    layoutGenerator.OnSimulatedAnnealingEvent -= SimulatedAnnealingEventHandler;

                    var additionalData = new AdditionalRunData()
                    {
                        SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                        GeneratedLayout = layout,
                    };

                    var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                    return generatorRun;
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 20);
            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResult(scenarioResult);

            var directory = $"CorridorConfigurationSpaces/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}";
            Directory.CreateDirectory(directory);

            var dataVisualization = new ChainStatsVisualization<GeneratorData>();
            foreach (var inputResult in scenarioResult.InputResults)
            {
                using (var file = new StreamWriter($"{directory}/{inputResult.InputName}.txt"))
                {
                    var generatorEvaluation = new GeneratorEvaluation(inputResult.Runs.Cast<IGeneratorRun<AdditionalRunData>>().ToList()); // TODO: ugly
                    dataVisualization.Visualize(generatorEvaluation, file);
                }
            }

            Utils.BenchmarkUtils.IsEqualToReference(scenarioResult, "BenchmarkResults/1577703636_CorridorConfigurationSpaces_Reference.json");
        }

        private static List<IRoomTemplate> GetBasicRoomTemplates(IntVector2 scale)
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

        private static List<IRoomTemplate> GetCorridorRoomTemplates(List<int> offsets)
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
                    new SpecificPositionsMode(new List<OrthogonalLine>()
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

        private static MapDescription<int> GetBasicMapDescription(IGraph<int> graph,
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

        public static List<DungeonGeneratorInput<int>> GetMapDescriptionsSet(IntVector2 scale, bool withCorridors, List<int> offsets, bool canTouch)
        {
            var basicRoomTemplates = GetBasicRoomTemplates(scale);
            var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

            var corridorRoomTemplates = GetCorridorRoomTemplates(offsets);
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

            var inputs = new List<DungeonGeneratorInput<int>>();

            {
                var mapDescription = GetBasicMapDescription(GraphsDatabase.GetExample1(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration(mapDescription) { RoomsCanTouch = canTouch };
                inputs.Add(new DungeonGeneratorInput<int>(
                    GetInputName("Example 1 (fig. 1)", scale, withCorridors, offsets, canTouch),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = GetBasicMapDescription(GraphsDatabase.GetExample2(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration(mapDescription) { RoomsCanTouch = canTouch };
                inputs.Add(new DungeonGeneratorInput<int>(
                    GetInputName("Example 2 (fig. 7 top)", scale, withCorridors, offsets, canTouch),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = GetBasicMapDescription(GraphsDatabase.GetExample3(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration(mapDescription) { RoomsCanTouch = canTouch };
                inputs.Add(new DungeonGeneratorInput<int>(
                    GetInputName("Example 3 (fig. 7 bottom)", scale, withCorridors, offsets, canTouch),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = GetBasicMapDescription(GraphsDatabase.GetExample4(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration(mapDescription) { RoomsCanTouch = canTouch };
                inputs.Add(new DungeonGeneratorInput<int>(
                    GetInputName("Example 4 (fig. 8)", scale, withCorridors, offsets, canTouch),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = GetBasicMapDescription(GraphsDatabase.GetExample5(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration(mapDescription) { RoomsCanTouch = canTouch };
                inputs.Add(new DungeonGeneratorInput<int>(
                    GetInputName("Example 5 (fig. 9)", scale, withCorridors, offsets, canTouch),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            return inputs;
        }

        private static string GetInputName(string name, IntVector2 scale, bool withCorridors, List<int> offsets, bool canTouch)
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