using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Common.LayoutControllers;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Graphs;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.GeneratorPlanners;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators;
using Edgar.Legacy.Core.LayoutGenerators.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Utils;
using ConfigurationSpacesGenerator = Edgar.GraphBasedGenerator.Grid2D.Internal.ConfigurationSpacesGenerator;

namespace Edgar.GraphBasedGenerator.RecursiveGrid2D.Internal
{
    /// <summary>
    /// Implements a graph-based layout generator that works on the 2D (integer) grid.
    /// </summary>
    public class InnerGenerator<TRoom> : IRandomInjectable, ICancellable,
        IObservableGenerator<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>>
    {
        private readonly ILevelDescription<RoomNode<TRoom>> levelDescription;
        private readonly LevelDescriptionGrid2D<RoomNode<TRoom>> levelDescriptionConfig;
        private readonly List<RoomNode<TRoom>> relevantRooms;
        private readonly Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>> initialLayout;
        private readonly LevelGeometryData<RoomNode<TRoom>> geometryData;
        private readonly Cluster<RoomNode<TRoom>> cluster;
        private readonly RoomTemplateInstanceGrid2D dummyRoomTemplateInstance;
        private readonly GraphBasedGeneratorConfiguration<RoomNode<TRoom>> configuration;

        private ChainBasedGenerator<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>,
            Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>> generator;

        // Exists because OnPerturbed converts layouts which uses the Random instance and causes results to be different.
        private event Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnPerturbedInternal;

        /// <summary>
        /// Creates an instance of the generator.
        /// </summary>
        /// <param name="levelDescription">Level description of the level that should be generated.</param>
        /// <param name="configuration">Configuration of the generator. Can be omitted for reasonable defaults.</param>
        public InnerGenerator(ILevelDescription<RoomNode<TRoom>> levelDescription,
            LevelDescriptionGrid2D<RoomNode<TRoom>> levelDescriptionConfig, List<RoomNode<TRoom>> relevantRooms,
            Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>> initialLayout,
            LevelGeometryData<RoomNode<TRoom>> geometryData, Cluster<RoomNode<TRoom>> cluster,
            RoomTemplateInstanceGrid2D dummyRoomTemplateInstance,
            GraphBasedGeneratorConfiguration<RoomNode<TRoom>> configuration = null)
        {
            this.levelDescription = levelDescription;
            this.levelDescriptionConfig = levelDescriptionConfig;
            this.relevantRooms = relevantRooms;
            this.initialLayout = initialLayout;
            this.geometryData = geometryData;
            this.cluster = cluster;
            this.dummyRoomTemplateInstance = dummyRoomTemplateInstance;
            this.configuration = configuration ?? new GraphBasedGeneratorConfiguration<RoomNode<TRoom>>();
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
            var levelDescriptionMapped = levelDescription;

            // Create configuration spaces generator
            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());

            // Preprocess information about room templates
            //var geometryData = LevelGeometryData<RoomNode<TRoom>>.CreateBackwardsCompatible(
            //    levelDescription,
            //    configurationSpacesGenerator.GetRoomTemplateInstances
            //);

            // Compute which rooms have fixed configurations
            var fixedConfigurationConstraint =
                GetFixedConfigurationConstraint(levelDescriptionConfig.Constraints, geometryData.RoomTemplateInstances);

            // Get chain decomposition algorithm
            var chainDecomposition = GraphBasedGeneratorGrid2DUtils.GetChainDecomposition(levelDescription,
                fixedConfigurationConstraint, configuration.ChainDecompositionConfiguration);

            var roomToAliasMapping = new TwoWayDictionary<RoomNode<TRoom>, RoomNode<TRoom>>();
            foreach (var vertex in levelDescription.GetGraph().Vertices)
            {
                roomToAliasMapping[vertex] = vertex;
            }

            var alreadyLaidOutRooms = this.initialLayout.Graph.Vertices
                .Where(x => this.initialLayout.GetConfiguration(x, out var _)).ToList();

            // Compute chains
            var relevantRooms = this.relevantRooms.Concat(alreadyLaidOutRooms)
                .Concat(cluster.Edges.Select(x => x.ToNode)).Distinct().ToList();
            var graph = GraphAlgorithms.GetInducedSubgraph(this.initialLayout.Graph, relevantRooms.ToHashSet(),
                new UndirectedAdjacencyListGraph<RoomNode<TRoom>>());
            var chains = GraphBasedGeneratorUtils.GetChains(
                chainDecomposition,
                graph,
                roomToAliasMapping,
                configuration.Chains
            );

            // Find out which rooms are not included in any chain
            // Such rooms must have both fixed shapes and position
            // Set these configurations accordingly in the initial layout
            var chainsSet = chains.SelectMany(x => x.Nodes).ToHashSet();
            var roomsWithoutChain = levelDescriptionMapped
                .GetGraph()
                .Vertices
                .Where(x => !chainsSet.Contains(x))
                .ToList();
            var initialLayout = this.initialLayout;
            //var initialLayout = new Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>(levelDescriptionMapped.GetGraph());
            //foreach (var room in roomsWithoutChain)
            //{
            //    initialLayout.SetConfiguration(room, new ConfigurationGrid2D<TRoom, EnergyData>()
            //    {
            //        Position = fixedConfigurationConstraint.GetFixedPosition(room),
            //        RoomShape = fixedConfigurationConstraint.GetFixeShape(room),
            //        Room = room,
            //        EnergyData = new EnergyData(),
            //    });
            //}

            // Create generator planner
            var generatorPlanner =
                new GeneratorPlanner<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>>(
                    configuration.SimulatedAnnealingMaxBranching);

            // The graph is directed if any of the doors is not Undirected
            var isGraphDirected = geometryData.RoomTemplateInstances
                .Values
                .SelectMany(x => x)
                .SelectMany(x => x.DoorLines)
                .Any(x => x.Type != DoorType.Undirected);


            var shapesForNodes =
                GraphBasedGeneratorGrid2DUtils.GetLegacyShapesForNodes(levelDescriptionMapped, geometryData);

            var averageRoomSize =
                GraphBasedGeneratorGrid2DUtils.GetLegacyAverageSize(levelDescriptionMapped, shapesForNodes);


            var configurationSpaces =
                new ConfigurationSpacesGrid2D<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>(
                    levelDescriptionMapped, null, isGraphDirected);

            var roomShapeGeometry =
                new FastGridPolygonGeometry<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>();

            var constraintsEvaluator = GraphBasedGeneratorGrid2DUtils.GetConstraintsEvaluator(levelDescriptionMapped,
                roomShapeGeometry, configurationSpaces, averageRoomSize, levelDescriptionConfig.MinimumRoomDistance,
                configuration.OptimizeCorridorConstraints);

            var roomShapesHandler =
                new RoomShapesHandlerGrid2D<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>(
                    geometryData.RoomTemplateInstanceToPolygonMapping,
                    levelDescriptionMapped,
                    shapesForNodes,
                    levelDescriptionConfig.RoomTemplateRepeatModeOverride,
                    levelDescriptionConfig.RoomTemplateRepeatModeDefault
                );

            // Create layout operations
            var layoutOperations =
                new LayoutController<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>,
                    ConfigurationGrid2D<TRoom, EnergyData>, RoomTemplateInstanceGrid2D, EnergyData>(averageRoomSize,
                    levelDescriptionMapped, constraintsEvaluator, roomShapesHandler,
                    configuration.ThrowIfRepeatModeNotSatisfied, configurationSpaces, roomShapeGeometry,
                    fixedConfigurationConstraint);


            var layoutConverter =
                new IdentityLayoutConverter<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>>();

            // Create simulated annealing evolver
            var layoutEvolver =
                new Common.SimulatedAnnealingEvolver<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>,
                    RoomNode<TRoom>,
                    ConfigurationGrid2D<TRoom, EnergyData>>(layoutOperations,
                    configuration.SimulatedAnnealingConfiguration, true);

            // Create the generator itself
            generator =
                new ChainBasedGenerator<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>,
                    Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>>(initialLayout,
                    generatorPlanner, chains, layoutEvolver, layoutConverter);

            // Register event handlers
            generator.OnRandomInjected += (random) =>
            {
                // ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable) layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable) layoutEvolver).InjectRandomGenerator(random);
                // ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
                ((IRandomInjectable) configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable) roomShapesHandler).InjectRandomGenerator(random);
            };

            generator.OnCancellationTokenInjected += (token) =>
            {
                ((ICancellable) generatorPlanner).SetCancellationToken(token);
                ((ICancellable) layoutEvolver).SetCancellationToken(token);
            };

            layoutEvolver.OnEvent += (sender, args) => OnSimulatedAnnealingEvent?.Invoke(sender, args);
            layoutEvolver.OnPerturbed +=
                (sender, layout) => OnPerturbed?.Invoke(layoutConverter.Convert(layout, false));
            layoutEvolver.OnPerturbed += (sender, layout) => OnPerturbedInternal?.Invoke(layout);
            layoutEvolver.OnValid += (sender, layout) => OnPartialValid?.Invoke(layoutConverter.Convert(layout, true));
            generatorPlanner.OnLayoutGenerated += layout => OnValid?.Invoke(layoutConverter.Convert(layout, true));
        }

        private FixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>
            GetFixedConfigurationConstraint(List<IGeneratorConstraintGrid2D<RoomNode<TRoom>>> constraints,
                Dictionary<RoomTemplateGrid2D, List<RoomTemplateInstanceGrid2D>> roomTemplateInstancesMapping)
        {
            var fixedPositions = new Dictionary<RoomNode<TRoom>, Vector2Int>();
            var fixedShapes = new Dictionary<RoomNode<TRoom>, RoomTemplateInstanceGrid2D>();

            foreach (var configuration in initialLayout.GetAllConfigurations())
            {
                // If the configuration is from the currents cluster, it must be the dummy room
                if (cluster.Nodes.Contains(configuration.Room))
                {
                    initialLayout.RemoveConfiguration(configuration.Room);
                }
                else
                {
                    fixedPositions[configuration.Room] = configuration.Position;
                    fixedShapes[configuration.Room] = configuration.RoomShape;
                }
            }

            // TODO: check for duplicate room constraints
            if (constraints != null)
            {
                throw new InvalidOperationException();
            }

            foreach (var edge in cluster.Edges)
            {
                if (fixedPositions.ContainsKey(edge.ToNode) == false)
                {
                    fixedShapes[edge.ToNode] = dummyRoomTemplateInstance;
                }
            }

            var constraint = new FixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>(
                levelDescription.GetGraph().VerticesCount, fixedShapes, fixedPositions);

            var graph = levelDescription.GetGraph();
            foreach (var room in graph.Vertices)
            {
                if (levelDescription.GetRoomDescription(room).IsCorridor &&
                    (constraint.IsFixedPosition(room) || constraint.IsFixedShape(room)))
                {
                    var neighbors = graph.GetNeighbors(room);

                    if (neighbors.Any(x => !constraint.IsFixedPosition(x) || !constraint.IsFixedShape(x)))
                    {
                        throw new InvalidOperationException(
                            "When a corridor room has a fixed configuration, all the neighboring rooms must have both fixed positions and shapes.");
                    }
                }
            }

            return constraint;
        }

        /// <summary>
        /// Generates a layout.
        /// </summary>
        /// <returns></returns>
        public Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>> GenerateLayout()
        {
            var earlyStoppingHandler = GetEarlyStoppingHandler(DateTime.Now);

            OnPerturbedInternal += earlyStoppingHandler;
            var layout = generator.GenerateLayout();
            OnPerturbedInternal -= earlyStoppingHandler;

            return layout;
        }

        private Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> GetEarlyStoppingHandler(
            DateTime generatorStarted)
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

                if (configuration.EarlyStopIfIterationsExceeded.HasValue &&
                    iterations > configuration.EarlyStopIfIterationsExceeded)
                {
                    cts.Cancel();
                }

                if (configuration.EarlyStopIfTimeExceeded.HasValue && iterations % 100 == 0 &&
                    DateTime.Now - generatorStarted > configuration.EarlyStopIfTimeExceeded)
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

        /// <summary>
        /// Injects an instance of the Random class. This makes it possible to get always the same results.
        /// </summary>
        /// <param name="random">Random numbers generator.</param>
        public void InjectRandomGenerator(Random random)
        {
            generator.InjectRandomGenerator(random);
        }

        /// <summary>
        /// Sets a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            if (configuration.EarlyStopIfIterationsExceeded.HasValue || configuration.EarlyStopIfTimeExceeded.HasValue)
            {
                throw new InvalidOperationException("Cannot use cancellation token when early stopping enabled");
            }

            generator.SetCancellationToken(cancellationToken);
        }

        public event Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnPerturbed;
        public event Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnPartialValid;
        public event Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnValid;
        public event EventHandler<SimulatedAnnealingEventArgs> OnSimulatedAnnealingEvent;
    }
}