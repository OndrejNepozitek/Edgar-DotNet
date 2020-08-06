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
        private ChainBasedGenerator<IMapDescription<int>, Layout<ConfigurationNew2<CorridorsDataNew>>, MapLayout<TNode>, int> generator;

        public event EventHandler<SimulatedAnnealingEventArgs> OnSimulatedAnnealingEvent;

        // Exists because OnPerturbed converts layouts which uses the Random instance and causes results to be different.
        private event Action<Layout<ConfigurationNew2<CorridorsDataNew>>> OnPerturbedInternal;

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
            var generatorPlanner = new GeneratorPlanner<Layout<ConfigurationNew2<CorridorsDataNew>>, int>(configuration.SimulatedAnnealingMaxBranching);

            // Create configuration spaces
            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());

            // var configurationSpaces = configurationSpacesGenerator.GetConfigurationSpaces<ConfigurationNew2<CorridorsDataNew>>(mapDescription);
            var simpleConfigurationSpaces = new ConfigurationSpacesGrid2D<ConfigurationNew2<CorridorsDataNew>, int>(mapDescription);

            // Needlessly complex for backwards compatibility

            #region IntAliasMapping

            var roomDescriptions = mapDescription.GetGraph().Vertices.ToDictionary(x => x, mapDescription.GetRoomDescription);
            var roomTemplates = roomDescriptions.Values.SelectMany(x => x.RoomTemplates).Distinct().ToList();
            var roomTemplateInstances = roomTemplates.ToDictionary(x => x, configurationSpacesGenerator.GetRoomTemplateInstances);
            var roomTemplateInstancesMapping = roomTemplateInstances.SelectMany(x => x.Value).CreateIntMapping();
            var intAliasMapping = new TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>>();

            foreach (var shape1 in roomTemplateInstancesMapping.Keys)
            {
                foreach (var shape2 in roomTemplateInstancesMapping.Keys)
                {
                    if (!intAliasMapping.ContainsKey(shape1))
                    {
                        var newAlias = new IntAlias<GridPolygon>(intAliasMapping.Count, shape1.RoomShape); 
                        intAliasMapping.Add(shape1, newAlias);
                        shape1.RoomShapeAlias = newAlias;
                    }
                    if (!intAliasMapping.ContainsKey(shape2))
                    {
                        var newAlias = new IntAlias<GridPolygon>(intAliasMapping.Count, shape2.RoomShape); 
                        intAliasMapping.Add(shape2, newAlias);
                        shape2.RoomShapeAlias = newAlias;
                    }
                }
            }

            // TODO: remove when possible
            foreach (var pair in intAliasMapping)
            {
                pair.Key.RoomShapeAlias = pair.Value;
            }

            var shapesForNodes = new Dictionary<int, List<WeightedShape>>();
            foreach (var vertex in mapDescription.GetGraph().Vertices)
            {
                shapesForNodes.Add(vertex, new List<WeightedShape>());
                var roomDescription = mapDescription.GetRoomDescription(vertex);

                foreach (var roomTemplate in roomDescription.RoomTemplates)
                {
                    var instances = roomTemplateInstances[roomTemplate];

                    foreach (var roomTemplateInstance in instances)
                    {
                        shapesForNodes[vertex].Add(new WeightedShape(intAliasMapping[roomTemplateInstance], 1d / instances.Count));
                    }
                }
            }

            var usedShapes = new HashSet<int>();
            var allShapes = new List<IntAlias<GridPolygon>>();
            foreach (var vertex in mapDescription.GetGraph().Vertices)
            {
                var shapes = shapesForNodes[vertex];

                foreach (var shape in shapes)
                {
                    if (!usedShapes.Contains(shape.Shape.Alias))
                    {
                        allShapes.Add(shape.Shape);
                        usedShapes.Add(shape.Shape.Alias);
                    }
                }
            }

            var averageSize = (int) allShapes.Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);

            #endregion



            // var averageSize = configurationSpaces.GetAverageSize();

            var energyUpdater = new BasicEnergyUpdater<int, ConfigurationNew2<CorridorsDataNew>>(10 * averageSize);
            var roomShapeGeometry = new FastGridPolygonGeometry<ConfigurationNew2<CorridorsDataNew>, int>();

            // Create generator constraints
            var stageOneConstraints =
                new List<INodeConstraint<ILayout<int, ConfigurationNew2<CorridorsDataNew>>, int, ConfigurationNew2<CorridorsDataNew>,
                    CorridorsDataNew>>
                {
                    new BasicConstraint<int, ConfigurationNew2<CorridorsDataNew>, CorridorsDataNew>(
                        roomShapeGeometry,
                        simpleConfigurationSpaces,
                        mapDescription,
                        configuration.OptimizeCorridorConstraints
                    ),
                    new CorridorConstraint<int, ConfigurationNew2<CorridorsDataNew>, CorridorsDataNew>(
                        mapDescription,
                        simpleConfigurationSpaces,
                        roomShapeGeometry
                    ),
                };

            if (levelDescriptionOriginal.MinimumRoomDistance > 0)
            {
                stageOneConstraints.Add(new MinimumDistanceConstraint<int, ConfigurationNew2<CorridorsDataNew>, CorridorsDataNew>(
                    mapDescription,
                    roomShapeGeometry,
                    levelDescriptionOriginal.MinimumRoomDistance
                ));
            }

            var constraintsEvaluator = new ConstraintsEvaluator<int, ConfigurationNew2<CorridorsDataNew>, CorridorsDataNew>(stageOneConstraints, energyUpdater);

            var roomShapesHandler = new RoomShapesHandler<int, ConfigurationNew2<CorridorsDataNew>>(
                intAliasMapping,
                mapDescription,
                shapesForNodes,
                configuration.RepeatModeOverride
            );

            // Create layout operations
            var layoutOperations = new LayoutController<Layout<ConfigurationNew2<CorridorsDataNew>>, int, ConfigurationNew2<CorridorsDataNew>, RoomTemplateInstance, CorridorsDataNew>(averageSize, mapDescription, constraintsEvaluator, roomShapesHandler, configuration.ThrowIfRepeatModeNotSatisfied, simpleConfigurationSpaces, roomShapeGeometry);

            var initialLayout = new Layout<ConfigurationNew2<CorridorsDataNew>>(mapDescription.GetGraph());
            var layoutConverter =
                new BasicLayoutConverterGrid2D<TNode,
                    ConfigurationNew2<CorridorsDataNew>>(mapDescription, simpleConfigurationSpaces,
                    intAliasMapping);

            // Create simulated annealing evolver
            var layoutEvolver =
                    new SimulatedAnnealingEvolver<Layout<ConfigurationNew2<CorridorsDataNew>>, int,
                    ConfigurationNew2<CorridorsDataNew>>(layoutOperations, configuration.SimulatedAnnealingConfiguration, true);

            // Create the generator itself
            generator = new ChainBasedGenerator<IMapDescription<int>, Layout<ConfigurationNew2<CorridorsDataNew>>, MapLayout<TNode>, int>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            // Register event handlers
            generator.OnRandomInjected += (random) =>
            {
                // ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
                ((IRandomInjectable)simpleConfigurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)roomShapesHandler).InjectRandomGenerator(random);
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

        private Action<Layout<ConfigurationNew2<CorridorsDataNew>>> GetEarlyStoppingHandler(DateTime generatorStarted)
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