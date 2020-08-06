using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.GraphBasedGenerator.Configurations;
using Edgar.GraphBasedGenerator.ConfigurationSpaces;
using Edgar.GraphBasedGenerator.Constraints;
using Edgar.GraphBasedGenerator.Constraints.BasicConstraint;
using Edgar.GraphBasedGenerator.Constraints.CorridorConstraint;
using Edgar.GraphBasedGenerator.Constraints.MinimumDistanceConstraint;
using Edgar.GraphBasedGenerator.RoomShapeGeometry;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.Configurations;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.GeneratorPlanners;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators;
using MapGeneration.Core.LayoutGenerators.Interfaces;
using MapGeneration.Core.LayoutOperations;
using MapGeneration.Core.Layouts;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator
{
    /// <summary>
    /// Implementation of the procedural dungeon generator algorithm.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class GraphBasedGenerator<TNode> : IRandomInjectable, ICancellable, IObservableGenerator<MapLayout<TNode>>
    {
        private readonly MapDescriptionMapping<TNode> mapDescription;
        private readonly LevelDescriptionGrid2D<TNode> levelDescriptionOriginal;
        private readonly GraphBasedGeneratorConfiguration<TNode> configuration;
        private ChainBasedGenerator<IMapDescription<int>, Layout<ConfigurationNew<CorridorsDataNew>>, MapLayout<TNode>, int> generator;

        public event EventHandler<SimulatedAnnealingEventArgs> OnSimulatedAnnealingEvent;

        // Exists because OnPerturbed converts layouts which uses the Random instance and causes results to be different.
        private event Action<Layout<ConfigurationNew<CorridorsDataNew>>> OnPerturbedInternal;

        public GraphBasedGenerator(LevelDescriptionGrid2D<TNode> levelDescription, GraphBasedGeneratorConfiguration<TNode> configuration = null)
        {
            this.levelDescriptionOriginal = levelDescription;
            this.mapDescription = new MapDescriptionMapping<TNode>(levelDescription);
            this.configuration = configuration ?? new GraphBasedGeneratorConfiguration<TNode>();
            SetupGenerator();
        }

        /// <summary>
        /// Total time to generate a level.
        /// </summary>
        public double TimeTotal => generator.TimeTotal;

        /// <summary>
        /// Number of iterations needed to generate the last level.
        /// </summary>
        public int IterationsCount => generator.IterationsCount;

        private void SetupGenerator()
        {
            var mapping = mapDescription.GetMapping();
            var chainsGeneric = configuration.Chains;

            // Create chain decomposition
            if (chainsGeneric == null)
            {
                var chainDecomposition = new TwoStageChainDecomposition<TNode>(levelDescriptionOriginal, new BreadthFirstChainDecomposition<TNode>(configuration.ChainDecompositionConfiguration ?? new ChainDecompositionConfiguration()));
                chainsGeneric = chainDecomposition.GetChains(levelDescriptionOriginal.GetGraph());
            }

            var chains = chainsGeneric
                .Select(x => new Chain<int>(x.Nodes.Select(y => mapping[y]).ToList(), x.Number){ IsFromFace = x.IsFromFace})
                .ToList();

            // Create generator planner
            var generatorPlanner = new GeneratorPlanner<Layout<ConfigurationNew<CorridorsDataNew>>, int>(configuration.SimulatedAnnealingMaxBranching);

            // Create configuration spaces
            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());

            var configurationSpaces = configurationSpacesGenerator.GetConfigurationSpaces<ConfigurationNew<CorridorsDataNew>>(mapDescription);
            var simpleConfigurationSpaces = new ConfigurationSpacesGrid2D<ConfigurationNew<CorridorsDataNew>, int>(configurationSpaces, mapDescription);

            // TODO: remove when possible
            foreach (var pair in configurationSpaces.GetIntAliasMapping())
            {
                pair.Key.RoomShapeAlias = pair.Value;
            }

            var averageSize = configurationSpaces.GetAverageSize();

            var energyUpdater = new BasicEnergyUpdater<int, ConfigurationNew<CorridorsDataNew>>(10 * averageSize);
            var roomShapeGeometry = new FastGridPolygonGeometry<ConfigurationNew<CorridorsDataNew>, int>();

            // Create generator constraints
            var stageOneConstraints =
                new List<INodeConstraint<ILayout<int, ConfigurationNew<CorridorsDataNew>>, int, ConfigurationNew<CorridorsDataNew>,
                    CorridorsDataNew>>
                {
                    new BasicConstraint<int, ConfigurationNew<CorridorsDataNew>, CorridorsDataNew>(
                        roomShapeGeometry,
                        simpleConfigurationSpaces,
                        mapDescription,
                        configuration.OptimizeCorridorConstraints
                    ),
                    new CorridorConstraint<int, ConfigurationNew<CorridorsDataNew>, CorridorsDataNew>(
                        mapDescription,
                        simpleConfigurationSpaces,
                        roomShapeGeometry
                    ),
                };

            if (levelDescriptionOriginal.MinimumRoomDistance > 0)
            {
                stageOneConstraints.Add(new MinimumDistanceConstraint<int, ConfigurationNew<CorridorsDataNew>, CorridorsDataNew>(
                    mapDescription,
                    roomShapeGeometry,
                    levelDescriptionOriginal.MinimumRoomDistance
                ));
            }

            var constraintsEvaluator = new ConstraintsEvaluator<int, ConfigurationNew<CorridorsDataNew>, CorridorsDataNew>(stageOneConstraints, energyUpdater);

            var roomShapesHandler = new RoomShapesHandler2<int, ConfigurationNew<CorridorsDataNew>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription,
                configuration.RepeatModeOverride
            );

            // Create layout operations
            var layoutOperations = new LayoutController<Layout<ConfigurationNew<CorridorsDataNew>>, int, ConfigurationNew<CorridorsDataNew>, IntAlias<GridPolygon>, CorridorsDataNew>(configurationSpaces.GetAverageSize(), mapDescription, constraintsEvaluator, roomShapesHandler, configuration.ThrowIfRepeatModeNotSatisfied, simpleConfigurationSpaces, roomShapeGeometry);

            var initialLayout = new Layout<ConfigurationNew<CorridorsDataNew>>(mapDescription.GetGraph());
            var layoutConverter =
                new BasicLayoutConverterGrid2D<TNode,
                    ConfigurationNew<CorridorsDataNew>>(mapDescription, simpleConfigurationSpaces,
                    configurationSpaces.GetIntAliasMapping());

            // Create simulated annealing evolver
            var layoutEvolver =
                    new SimulatedAnnealingEvolver<Layout<ConfigurationNew<CorridorsDataNew>>, int,
                    ConfigurationNew<CorridorsDataNew>>(layoutOperations, configuration.SimulatedAnnealingConfiguration, true);

            // Create the generator itself
            generator = new ChainBasedGenerator<IMapDescription<int>, Layout<ConfigurationNew<CorridorsDataNew>>, MapLayout<TNode>, int>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            // Register event handlers
            generator.OnRandomInjected += (random) =>
            {
                ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
                ((IRandomInjectable)simpleConfigurationSpaces).InjectRandomGenerator(random);
            };

            generator.OnCancellationTokenInjected += (token) =>
            {
                ((ICancellable)generatorPlanner).SetCancellationToken(token);
                ((ICancellable)layoutEvolver).SetCancellationToken(token);
            };
            
            layoutEvolver.OnEvent += (sender, args) => OnSimulatedAnnealingEvent?.Invoke(sender, args);
            layoutEvolver.OnPerturbed += (sender, layout) => OnPerturbed?.Invoke(layoutConverter.Convert(layout, false));
            layoutEvolver.OnPerturbed += (sender, layout) => OnPerturbedInternal?.Invoke(layout);
            layoutEvolver.OnValid += (sender, layout) => OnPartialValid?.Invoke(layoutConverter.Convert(layout, true));
            generatorPlanner.OnLayoutGenerated += layout => OnValid?.Invoke(layoutConverter.Convert(layout, true));
        }

        /// <summary>
        /// Generates a level.
        /// </summary>
        /// <returns></returns>
        public MapLayout<TNode> GenerateLayout()
        {
            var earlyStoppingHandler = GetEarlyStoppingHandler(DateTime.Now);

            OnPerturbedInternal += earlyStoppingHandler;
            var layout = generator.GenerateLayout();
            OnPerturbedInternal -= earlyStoppingHandler;

            return layout;
        }

        private Action<Layout<ConfigurationNew<CorridorsDataNew>>> GetEarlyStoppingHandler(DateTime generatorStarted)
        {
            var iterations = 0;
            var cts = new CancellationTokenSource();

            if (IsEarlyStoppingEnabled())
            {
                generator.SetCancellationToken(cts.Token);
            }

            return layout =>
            {
                iterations++;

                if (configuration.EarlyStopIfIterationsExceeded.HasValue && iterations > configuration.EarlyStopIfIterationsExceeded)
                {
                    cts.Cancel();
                }

                if (configuration.EarlyStopIfTimeExceeded.HasValue && iterations % 100 == 0 && DateTime.Now - generatorStarted > configuration.EarlyStopIfTimeExceeded)
                {
                    cts.Cancel();
                }
            };
        }

        private bool IsEarlyStoppingEnabled()
        {
            return configuration.EarlyStopIfIterationsExceeded.HasValue ||
                   configuration.EarlyStopIfTimeExceeded.HasValue;
        }

        public void InjectRandomGenerator(Random random)
        {
            generator.InjectRandomGenerator(random);
        }

        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            if (configuration.EarlyStopIfIterationsExceeded.HasValue || configuration.EarlyStopIfTimeExceeded.HasValue)
            {
                throw new InvalidOperationException("Cannot use cancellation token when early stopping enabled");
            }

            generator.SetCancellationToken(cancellationToken);
        }

        public event Action<MapLayout<TNode>> OnPerturbed;
        public event Action<MapLayout<TNode>> OnPartialValid;
        public event Action<MapLayout<TNode>> OnValid;
    }
}