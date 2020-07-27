using System;
using System.Collections.Generic;
using System.IO;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.Utils.MapDrawing;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class OneImprovementDisabled : Scenario
    {
        private DungeonGeneratorConfiguration<int> GetWithoutGreedyTreesConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetNewConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration.GetConfiguration(0).HandleTreesGreedily = false;

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetWithoutMaxIterationsConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetNewConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 10000,
            });

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetWithoutChainDecompositionConfiguration(NamedMapDescription namedMapDescription)
        {
            var chainDecompositionOld = new BreadthFirstChainDecompositionOld<int>();
            var chainDecomposition = new TwoStageChainDecomposition<int>(namedMapDescription.MapDescription, chainDecompositionOld);

            var configuration = GetNewConfiguration(namedMapDescription);
            configuration.Chains = chainDecomposition.GetChains(namedMapDescription.MapDescription.GetGraph());

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetNewConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
                HandleTreesGreedily = true,
            });

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetOldConfiguration(NamedMapDescription namedMapDescription)
        {
            var chainDecompositionOld = new BreadthFirstChainDecompositionOld<int>();
            var chainDecomposition = new TwoStageChainDecomposition<int>(namedMapDescription.MapDescription, chainDecompositionOld);

            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.Chains = chainDecomposition.GetChains(namedMapDescription.MapDescription.GetGraph());
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 10000,
            });

            return configuration;
        }

        protected override void Run()
        {
            var mapDescriptions = GetMapDescriptions();

            RunBenchmark(mapDescriptions, GetNewConfiguration, Options.FinalEvaluationIterations, "New");
            RunBenchmark(mapDescriptions, GetOldConfiguration, Options.FinalEvaluationIterations, "Old");
            //RunBenchmark(mapDescriptions, GetWithoutGreedyTreesConfiguration, Options.FinalEvaluationIterations, "WithoutGreedyTrees");
            //RunBenchmark(mapDescriptions, GetWithoutMaxIterationsConfiguration, Options.FinalEvaluationIterations, "WithoutMaxIterations");
            //RunBenchmark(mapDescriptions, GetWithoutChainDecompositionConfiguration, Options.FinalEvaluationIterations, "WithoutChainDecomposition");
        }

        protected override DungeonGeneratorConfiguration<int> GetBasicConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = base.GetBasicConfiguration(namedMapDescription);
            configuration.RepeatModeOverride = RepeatMode.NoRepeat;
            return configuration;
        }

        /*protected override IGeneratorRunner GetGeneratorRunnerFactory(GeneratorInput<IMapDescription<int>> input)
        {
            var layoutDrawer = new SVGLayoutDrawer<int>();

            var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration);
            layoutGenerator.InjectRandomGenerator(new Random(0));

            Logger.WriteLine($"{input.Name} {input.MapDescription.GetStageOneGraph().VerticesCount}");

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

                var additionalData = new AdditionalRunData<int>()
                {
                    SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                    GeneratedLayoutSvg = layout != null ? layoutDrawer.DrawLayout(layout, 800, forceSquare: true) : null,
                    GeneratedLayout = layout,
                };

                var path = Path.Combine(DirectoryFullPath, $"{input.Name}.svg");
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, layoutDrawer.DrawLayout(layout, 800, forceSquare: false, flipY: true, showRoomNames: false));
                }
                
                var generatorRun = new GeneratorRun<AdditionalRunData<int>>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                return generatorRun;
            });
        }*/
    }
}