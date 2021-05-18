using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Constraints;
using Edgar.GraphBasedGenerator.Common.Constraints.BasicConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.CorridorConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.MinimumDistanceConstraint;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Common.LayoutControllers;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.GeneratorPlanners;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators;
using Edgar.Legacy.Core.LayoutGenerators.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;
using ConfigurationSpacesGenerator = Edgar.GraphBasedGenerator.Grid2D.Internal.ConfigurationSpacesGenerator;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Implements a graph-based layout generator that works on the 2D (integer) grid.
    /// </summary>
    public class GraphBasedGeneratorGrid2D<TRoom> : IRandomInjectable, ICancellable, IObservableGenerator<LayoutGrid2D<TRoom>>
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
        /// Total time to generate a level.
        /// </summary>
        public double TimeTotal => generator.TimeTotal;

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
            var fixedRooms = levelDescriptionMapped
                .GetGraphWithoutCorridors()
                .Vertices
                .Where(x => fixedConfigurationConstraint.IsFixedPosition(x) && fixedConfigurationConstraint.IsFixedShape(x))
                .ToList();

            // Compute chain decomposition
            var chainDecompositionConfiguration = configuration.ChainDecompositionConfiguration ?? new ChainDecompositionConfiguration();
            var chainDecomposition = new BreadthFirstChainDecomposition<RoomNode<TRoom>>(chainDecompositionConfiguration);
            var twoStageChainDecomposition =  new Common.ChainDecomposition.TwoStageChainDecomposition<RoomNode<TRoom>>(
                levelDescriptionMapped,
                chainDecomposition
            );
            var chains = GraphBasedGeneratorUtils.GetChains(
                twoStageChainDecomposition,
                levelDescriptionMapped.GetGraph(),
                roomToAliasMapping,
                configuration.Chains
            );

            // Create generator planner
            var generatorPlanner = new GeneratorPlanner<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>>(configuration.SimulatedAnnealingMaxBranching);

            // The graph is directed if any of the doors is not Undirected
            var isGraphDirected = geometryData.RoomTemplateInstances
                .Values
                .SelectMany(x => x)
                .SelectMany(x => x.DoorLines)
                .Any(x => x.Type != DoorType.Undirected);

            // Needlessly complex for backwards compatibility

            #region IntAliasMapping

            var shapesForNodes = new Dictionary<RoomNode<TRoom>, List<WeightedShape>>();
            foreach (var vertex in levelDescriptionMapped.GetGraph().Vertices)
            {
                shapesForNodes.Add(vertex, new List<WeightedShape>());
                // var roomDescription = levelDescriptionMapped.GetRoomDescription(vertex);
                var roomDescription = geometryData.RoomDescriptions[vertex];

                foreach (var roomTemplate in roomDescription.RoomTemplates)
                {
                    var roomTemplateInstances = geometryData.RoomTemplateInstances[roomTemplate];

                    foreach (var roomTemplateInstance in roomTemplateInstances)
                    {
                        shapesForNodes[vertex].Add(new WeightedShape(
                            geometryData.RoomTemplateInstanceToPolygonMapping[roomTemplateInstance],
                            1d / roomTemplateInstances.Count)
                        );
                    }
                }
            }

            var usedShapes = new HashSet<int>();
            var allShapes = new List<IntAlias<PolygonGrid2D>>();
            foreach (var vertex in levelDescriptionMapped.GetGraph().Vertices)
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



            // var configurationSpaces = configurationSpacesGenerator.GetConfigurationSpaces<ConfigurationNew2<CorridorsDataNew>>(mapDescription);
            var simpleConfigurationSpaces = new ConfigurationSpacesGrid2D<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>(levelDescriptionMapped, null, isGraphDirected);

            // var averageSize = configurationSpaces.GetAverageSize();

            var energyUpdater = new BasicEnergyUpdater<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>(10 * averageSize);
            var roomShapeGeometry = new FastGridPolygonGeometry<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>();

            // Create generator constraints
            var stageOneConstraints =
                new List<INodeConstraint<ILayout<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>,
                    EnergyData>>
                {
                    new BasicConstraint<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(
                        roomShapeGeometry,
                        simpleConfigurationSpaces,
                        levelDescriptionMapped,
                        configuration.OptimizeCorridorConstraints
                    ),
                    new CorridorConstraint<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(
                        levelDescriptionMapped,
                        simpleConfigurationSpaces,
                        roomShapeGeometry
                    ),
                };

            if (levelDescription.MinimumRoomDistance > 0)
            {
                stageOneConstraints.Add(new MinimumDistanceConstraint<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(
                    levelDescriptionMapped,
                    roomShapeGeometry,
                    levelDescription.MinimumRoomDistance
                ));
            }

            var constraintsEvaluator = new ConstraintsEvaluator<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(stageOneConstraints, energyUpdater);

            var roomShapesHandler = new RoomShapesHandlerGrid2D<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>(
                geometryData.RoomTemplateInstanceToPolygonMapping,
                levelDescriptionMapped,
                shapesForNodes,
                levelDescription.RoomTemplateRepeatModeOverride,
                levelDescription.RoomTemplateRepeatModeDefault
            );

            // Create layout operations
            var layoutOperations = new LayoutController<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, RoomTemplateInstanceGrid2D, EnergyData>(averageSize, levelDescriptionMapped, constraintsEvaluator, roomShapesHandler, configuration.ThrowIfRepeatModeNotSatisfied, simpleConfigurationSpaces, roomShapeGeometry, fixedConfigurationConstraint);

            var initialLayout = new Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>(levelDescriptionMapped.GetGraph());
            var layoutConverter =
                new BasicLayoutConverterGrid2D<TRoom,
                    ConfigurationGrid2D<TRoom, EnergyData>>(levelDescription, simpleConfigurationSpaces,
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

        private FixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom> GetFixedConfigurationConstraint(List<IGeneratorConstraintGrid2D<TRoom>> constraints, Dictionary<RoomTemplateGrid2D, List<RoomTemplateInstanceGrid2D>> roomTemplateInstancesMapping)
        {
            var fixedPositions = new Dictionary<RoomNode<TRoom>, Vector2Int>();
            var fixedShapes = new Dictionary<RoomNode<TRoom>, RoomTemplateInstanceGrid2D>();
            var mapping = levelDescriptionMapped.GetMapping();

            // TODO: check for duplicate room constraints
            if (constraints != null)
            {
                foreach (var constraint in constraints)
                {
                    if (constraint is FixedConfigurationConstraint<TRoom> fixedConfigurationConstraint)
                    {
                        if (!mapping.TryGetValue(fixedConfigurationConstraint.Room, out var roomNode))
                        {
                            throw new InvalidOperationException(
                                $"FixedConfigurationConstraint contained a room that is not present in the level description. The room was: {fixedConfigurationConstraint.Room}");
                        }

                        if (levelDescriptionMapped.GetRoomDescription(roomNode).IsCorridor)
                        {
                            throw new InvalidOperationException(
                                $"FixedConfigurationConstraint currently works only for non-corridor rooms.");
                        }

                        if (fixedConfigurationConstraint.Position.HasValue)
                        {
                            fixedPositions[roomNode] = fixedConfigurationConstraint.Position.Value;
                        }

                        // TODO: check for not existing room template instance
                        if (fixedConfigurationConstraint.RoomTemplate != null)
                        {
                            // TODO: handle transformations
                            var roomTemplateInstance =
                                roomTemplateInstancesMapping[fixedConfigurationConstraint.RoomTemplate].Single(x =>
                                    x.Transformations.Contains(TransformationGrid2D.Identity));

                            fixedShapes[roomNode] = roomTemplateInstance;
                        }
                    }
                }
            }

            return new FixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>(
                mapping.Count, fixedShapes, fixedPositions);
        }

        /// <summary>
        /// Generates a layout.
        /// </summary>
        /// <returns></returns>
        public LayoutGrid2D<TRoom> GenerateLayout()
        {
            var earlyStoppingHandler = GetEarlyStoppingHandler(DateTime.Now);

            OnPerturbedInternal += earlyStoppingHandler;
            var layout = generator.GenerateLayout();
            OnPerturbedInternal -= earlyStoppingHandler;

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