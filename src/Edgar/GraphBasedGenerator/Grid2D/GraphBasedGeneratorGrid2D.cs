using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.Benchmarks;
using Edgar.Benchmarks.Interfaces;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Common.LayoutControllers;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.GeneratorPlanners;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators;
using Edgar.Legacy.Core.LayoutGenerators.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Utils;
using ConfigurationSpacesGenerator = Edgar.GraphBasedGenerator.Grid2D.Internal.ConfigurationSpacesGenerator;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Implements a graph-based layout generator that works on the 2D (integer) grid.
    /// </summary>
    public class GraphBasedGeneratorGrid2D<TRoom> : IRandomInjectable, ICancellable, IObservableGenerator<LayoutGrid2D<TRoom>>, IBenchmarkableLayoutGenerator<LayoutGrid2D<TRoom>>
    {
        private readonly LevelDescriptionMapping<TRoom> levelDescriptionMapped;
        private readonly LevelDescriptionGrid2D<TRoom> levelDescription;
        private readonly GraphBasedGeneratorConfiguration<TRoom> configuration;
        private ChainBasedGenerator<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, LayoutGrid2D<TRoom>, RoomNode<TRoom>> generator;
        
        // Exists because OnPerturbed converts layouts which uses the Random instance and causes results to be different.
        private event Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnPerturbedInternal;

        /// <summary>
        /// Creates an instance of the generator.
        /// </summary>
        /// <param name="levelDescription">Level description of the level that should be generated.</param>
        /// <param name="configuration">Configuration of the generator. Can be omitted for reasonable defaults.</param>
        public GraphBasedGeneratorGrid2D(LevelDescriptionGrid2D<TRoom> levelDescription, GraphBasedGeneratorConfiguration<TRoom> configuration = null)
        {
            this.levelDescription = levelDescription;
            this.levelDescriptionMapped = new LevelDescriptionMapping<TRoom>(levelDescription);
            this.configuration = configuration ?? new GraphBasedGeneratorConfiguration<TRoom>();
            SetupGenerator();
        }

        /// <summary>
        /// Number of milliseconds needed to generate the last level.
        /// </summary>
        public long TimeTotal => generator.TimeTotal;

        /// <summary>
        /// Number of iterations needed to generate the last level.
        /// </summary>
        public int IterationsCount => generator.IterationsCount;

        private void SetupGenerator()
        {
            var roomToAliasMapping = levelDescriptionMapped.GetMapping();

            // Create configuration spaces generator
            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());

            // Preprocess information about room templates
            var geometryData = LevelGeometryData<RoomNode<TRoom>>.CreateBackwardsCompatible(
                levelDescriptionMapped,
                configurationSpacesGenerator.GetRoomTemplateInstances
            );

            // Compute which rooms have fixed configurations
            var fixedConfigurationConstraint =
                GetFixedConfigurationConstraint(levelDescription.Constraints, geometryData.RoomTemplateInstances);

            // Get chain decomposition algorithm
            var chainDecomposition = GraphBasedGeneratorGrid2DUtils.GetChainDecomposition(levelDescriptionMapped,
                fixedConfigurationConstraint, configuration.ChainDecompositionConfiguration);

            // Compute chains
            var chains = GraphBasedGeneratorUtils.GetChains(
                chainDecomposition,
                levelDescriptionMapped.GetGraph(),
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
            var initialLayout = new Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>(levelDescriptionMapped.GetGraph());
            foreach (var room in roomsWithoutChain)
            {
                initialLayout.SetConfiguration(room, new ConfigurationGrid2D<TRoom, EnergyData>()
                {
                    Position = fixedConfigurationConstraint.GetFixedPosition(room),
                    RoomShape = fixedConfigurationConstraint.GetFixeShape(room),
                    Room = room,
                    EnergyData = new EnergyData(),
                });
            }

            // Create generator planner
            var generatorPlanner = new GeneratorPlanner<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>>(configuration.SimulatedAnnealingMaxBranching);

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


            var configurationSpaces = new ConfigurationSpacesGrid2D<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>(levelDescriptionMapped, null, isGraphDirected);

            var roomShapeGeometry = new FastGridPolygonGeometry<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>();

            var constraintsEvaluator = GraphBasedGeneratorGrid2DUtils.GetConstraintsEvaluator(levelDescriptionMapped,
                roomShapeGeometry, configurationSpaces, averageRoomSize, levelDescription.MinimumRoomDistance,
                configuration.OptimizeCorridorConstraints);

            var roomShapesHandler = new RoomShapesHandlerGrid2D<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>(
                geometryData.RoomTemplateInstanceToPolygonMapping,
                levelDescriptionMapped,
                shapesForNodes,
                levelDescription.RoomTemplateRepeatModeOverride,
                levelDescription.RoomTemplateRepeatModeDefault
            );

            // Create layout operations
            var layoutOperations = new LayoutController<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, RoomTemplateInstanceGrid2D, EnergyData>(averageRoomSize, levelDescriptionMapped, constraintsEvaluator, roomShapesHandler, configuration.ThrowIfRepeatModeNotSatisfied, configurationSpaces, roomShapeGeometry, fixedConfigurationConstraint);

            
            var layoutConverter =
                new BasicLayoutConverterGrid2D<TRoom,
                    ConfigurationGrid2D<TRoom, EnergyData>>(levelDescription, configurationSpaces,
                    geometryData.RoomTemplateInstanceToPolygonMapping);

            // Create simulated annealing evolver
            var layoutEvolver =
                    new Common.SimulatedAnnealingEvolver<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>,
                    ConfigurationGrid2D<TRoom, EnergyData>>(layoutOperations, configuration.SimulatedAnnealingConfiguration, true);

            // Create the generator itself
            generator = new ChainBasedGenerator<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, LayoutGrid2D<TRoom>, RoomNode<TRoom>>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            // Register event handlers
            generator.OnRandomInjected += (random) =>
            {
                // ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
                ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
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

        private FixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom> GetFixedConfigurationConstraint(List<IGeneratorConstraintGrid2D<TRoom>> constraints, Dictionary<RoomTemplateGrid2D, List<RoomTemplateInstanceGrid2D>> roomTemplateInstancesMapping)
        {
            var fixedPositions = new Dictionary<RoomNode<TRoom>, Vector2Int>();
            var fixedShapes = new Dictionary<RoomNode<TRoom>, RoomTemplateInstanceGrid2D>();
            var mapping = levelDescriptionMapped.GetMapping();

            // TODO: check for duplicate room constraints
            if (constraints != null)
            {
                foreach (var constraintData in constraints)
                {
                    if (constraintData is FixedConfigurationConstraint<TRoom> fixedConfigurationConstraint)
                    {
                        if (!mapping.TryGetValue(fixedConfigurationConstraint.Room, out var roomNode))
                        {
                            throw new InvalidOperationException(
                                $"FixedConfigurationConstraint contained a room that is not present in the level description. The room was: {fixedConfigurationConstraint.Room}");
                        }

                        // TODO: should it be possible to lock the position without locking the shape?
                        //if (fixedConfigurationConstraint.Position.HasValue)
                        //{
                        //    fixedPositions[roomNode] = fixedConfigurationConstraint.Position.Value;
                        //}

                        // TODO: check for not existing room template instance
                        if (fixedConfigurationConstraint.RoomTemplate != null)
                        {
                            // TODO: handle transformations
                            // TODO: handle transformation not exists
                            var roomTemplateInstance =
                                roomTemplateInstancesMapping[fixedConfigurationConstraint.RoomTemplate].Single(x =>
                                    x.Transformations.Contains(fixedConfigurationConstraint.Transformation));

                            fixedShapes[roomNode] = roomTemplateInstance;

                            if (fixedConfigurationConstraint.Position.HasValue)
                            {
                                var transformedShape = roomTemplateInstance.RoomTemplate.Outline.Transform(fixedConfigurationConstraint.Transformation);
                                var offset = roomTemplateInstance.RoomShape.BoundingRectangle.A - transformedShape.BoundingRectangle.A;
                                fixedPositions[roomNode] = fixedConfigurationConstraint.Position.Value - offset;
                            }
                        }
                        else
                        {
                            // TODO: improve
                            throw new InvalidOperationException(
                                $"RoomTemplate must not be null");
                        }
                    }
                }
            }

            var constraint = new FixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>(
                mapping.Count, fixedShapes, fixedPositions);

            var graph = levelDescriptionMapped.GetGraph();
            foreach (var room in graph.Vertices)
            {
                if (levelDescriptionMapped.GetRoomDescription(room).IsCorridor && (constraint.IsFixedPosition(room) || constraint.IsFixedShape(room)))
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
        public LayoutGrid2D<TRoom> GenerateLayout()
        {
            return GenerateLayout(out _);
        }

        public LayoutGrid2D<TRoom> GenerateLayout(out IGeneratorRun runData)
        {
            var earlyStoppingHandler = GetEarlyStoppingHandler(DateTime.Now);

            OnPerturbedInternal += earlyStoppingHandler;
            var layout = generator.GenerateLayout();
            OnPerturbedInternal -= earlyStoppingHandler;

            runData = new GeneratorRun(layout != null, TimeTotal, IterationsCount);

            return layout;
        }

        private Action<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> GetEarlyStoppingHandler(DateTime generatorStarted)
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

        public event Action<LayoutGrid2D<TRoom>> OnPerturbed;
        public event Action<LayoutGrid2D<TRoom>> OnPartialValid;
        public event Action<LayoutGrid2D<TRoom>> OnValid;
        public event EventHandler<SimulatedAnnealingEventArgs> OnSimulatedAnnealingEvent;
    }
}