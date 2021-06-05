﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.GraphBasedGenerator.RecursiveGrid2D;
using Edgar.Legacy.Benchmarks;
using Edgar.Legacy.Benchmarks.GeneratorRunners;
using Edgar.Legacy.Benchmarks.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Utils.MapDrawing;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators
{
    public class RecursiveGeneratorFactory<TNode> : ILevelGeneratorFactory<TNode>
    {
        private readonly GraphBasedGeneratorConfiguration<TNode> configuration;
        private readonly bool benchmarkInitialization;

        public string Name { get; }

        public RecursiveGeneratorFactory(GraphBasedGeneratorConfiguration<TNode> configuration, bool benchmarkInitialization = false, string name = null)
        {
            this.configuration = configuration;
            this.benchmarkInitialization = benchmarkInitialization;
            Name = $"{name ?? "New generator"}" + (benchmarkInitialization ? " with init" : "");
        }

        public IGeneratorRunner GetGeneratorRunner(LevelDescriptionGrid2D<TNode> levelDescription)
        {
            if (benchmarkInitialization)
            {
                return GetGeneratorRunnerWithInit(levelDescription);
            }

            var configuration = this.configuration.SmartClone();
            configuration.RoomsCanTouch = levelDescription.MinimumRoomDistance == 0;

            var layoutDrawer = new Edgar.GraphBasedGenerator.Grid2D.Drawing.SVGLayoutDrawer<TNode>();

            var layoutGenerator = new RecursiveGraphBasedGeneratorGrid2D<TNode>(levelDescription, configuration);
            layoutGenerator.InjectRandomGenerator(new Random(0));

            return new LambdaGeneratorRunner(() =>
            {
                var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();

                void SimulatedAnnealingEventHandler(object sender, SimulatedAnnealingEventArgs eventArgs)
                {
                    simulatedAnnealingArgsContainer.Add(eventArgs);
                }
                
                var dungeonDrawer = new DungeonDrawer<TNode>();

                void OnInvalidHandler(LayoutGrid2D<TNode> l)
                {
                    dungeonDrawer.DrawLayoutAndSave(l, $"invalid_{levelDescription.Name}_{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}.png", new DungeonDrawerOptions()
                    {
                        Width = 1000,
                        Height = 1000,
                    });
                }

                layoutGenerator.OnPartialValid += OnInvalidHandler;
                layoutGenerator.OnSimulatedAnnealingEvent += SimulatedAnnealingEventHandler;

                var layout = layoutGenerator.GenerateLayout();

                layoutGenerator.OnSimulatedAnnealingEvent -= SimulatedAnnealingEventHandler;
                layoutGenerator.OnPartialValid -= OnInvalidHandler;

                var additionalData = new AdditionalRunData<TNode>()
                {
                    SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                    GeneratedLayoutSvg =
                        layout != null ? layoutDrawer.DrawLayout(layout, 800, forceSquare: true) : null,
                    // GeneratedLayout = layout,
                };

                var generatorRun = new GeneratorRun<AdditionalRunData<TNode>>(layout != null, layoutGenerator.TimeTotal,
                    layoutGenerator.IterationsCount, additionalData);

                return generatorRun;
            });
        }

        private IGeneratorRunner GetGeneratorRunnerWithInit(LevelDescriptionGrid2D<TNode> levelDescription)
        {
            var configuration = this.configuration.SmartClone();
            configuration.RoomsCanTouch = levelDescription.MinimumRoomDistance == 0;

            var layoutDrawer = new Edgar.GraphBasedGenerator.Grid2D.Drawing.SVGLayoutDrawer<TNode>();
            var seedGenerator = new Random();

            return new LambdaGeneratorRunner(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var layoutGenerator = new RecursiveGraphBasedGeneratorGrid2D<TNode>(levelDescription, configuration);
                layoutGenerator.InjectRandomGenerator(new Random(seedGenerator.Next()));

                var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();

                void SimulatedAnnealingEventHandler(object sender, SimulatedAnnealingEventArgs eventArgs)
                {
                    simulatedAnnealingArgsContainer.Add(eventArgs);
                }

                layoutGenerator.OnSimulatedAnnealingEvent += SimulatedAnnealingEventHandler;
                var layout = layoutGenerator.GenerateLayout();
                layoutGenerator.OnSimulatedAnnealingEvent -= SimulatedAnnealingEventHandler;

                stopwatch.Stop();

                var additionalData = new AdditionalRunData<TNode>()
                {
                    SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                    //GeneratedLayoutSvg =
                    //    layout != null ? layoutDrawer.DrawLayout(layout, 800, forceSquare: true) : null,
                    // GeneratedLayout = layout,
                };

                var generatorRun = new GeneratorRun<AdditionalRunData<TNode>>(layout != null, stopwatch.ElapsedMilliseconds,
                    layoutGenerator.IterationsCount, additionalData);

                return generatorRun;
            });
        }
    }
}