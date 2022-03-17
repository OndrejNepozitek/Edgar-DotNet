﻿using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Scenarios
{
    public class DeformedRoomTemplates : Benchmark
    {
        private BenchmarkScenario<int> GetDeformedScenario(List<NamedGraph<int>> graphs)
        {
            var levelDescriptionLoader = new CustomLevelDescriptionLoader(RoomTemplatesSet.Smart, new Vector2Int(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(graphs, new List<int>() {0});

            return new BenchmarkScenario<int>("Deformed", levelDescriptions);
        }

        private BenchmarkScenario<int> GetNormalScenario(List<NamedGraph<int>> graphs)
        {
            var levelDescriptionLoader = new LevelDescriptionLoader(RoomTemplatesSet.Smart, new Vector2Int(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(graphs, new List<int>() {0});

            return new BenchmarkScenario<int>("Normal", levelDescriptions);
        }

        protected override void Run()
        {
            var graphs = GraphLoader.GetRandomGraphsVariety(20);
            var options = new BenchmarkOptions()
            {
                EarlyStopTime = 5000,
            };

            var scenarios = new List<BenchmarkScenario<int>>()
            {
                GetDeformedScenario(graphs),
                GetNormalScenario(graphs),
            };

            var generators = new List<ILevelGeneratorFactory<int>>()
            {
                GetOldGenerator<int>(options),
                GetNewGenerator<int>(options),
            };

            RunBenchmark(scenarios, generators);
        }

        public class CustomLevelDescriptionLoader : LevelDescriptionLoader
        {
            public CustomLevelDescriptionLoader(RoomTemplatesSet roomTemplatesSet, Vector2Int scale,
                RoomTemplateRepeatMode repeatMode = RoomTemplateRepeatMode.AllowRepeat) : base(roomTemplatesSet, scale,
                repeatMode)
            {
            }

            protected override RoomTemplateGrid2D GetRectangleRoomTemplate(int width, int height,
                SimpleDoorModeGrid2D doorMode)
            {
                if (doorMode.CornerDistance >= 2)
                {
                    var polygon = new PolygonGrid2DBuilder()
                        .AddPoint(2, 0)
                        .AddPoint(2, 1)
                        .AddPoint(1, 1)
                        .AddPoint(1, 2)
                        .AddPoint(0, 2)
                        .AddPoint(0, height - 2)
                        .AddPoint(1, height - 2)
                        .AddPoint(1, height - 1)
                        .AddPoint(2, height - 1)
                        .AddPoint(2, height)
                        .AddPoint(width - 2, height)
                        .AddPoint(width - 2, height - 1)
                        .AddPoint(width - 1, height - 1)
                        .AddPoint(width - 1, height - 2)
                        .AddPoint(width, height - 2)
                        .AddPoint(width, 2)
                        .AddPoint(width - 1, 2)
                        .AddPoint(width - 1, 1)
                        .AddPoint(width - 2, 1)
                        .AddPoint(width - 2, 0)
                        .Build();

                    return GetRoomTemplate(polygon,
                        new SimpleDoorModeGrid2D(doorMode.DoorLength, doorMode.CornerDistance - 2),
                        $"Deformed {width}x{height}");
                }
                else
                {
                    return base.GetRectangleRoomTemplate(width, height, doorMode);
                }
            }
        }
    }
}