using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.GraphBasedGenerator.RecursiveGrid2D.Internal;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.GeneratorPlanners;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators;
using Edgar.Legacy.Core.LayoutGenerators.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.RecursiveGrid2D
{
    public class RecursiveGraphBasedGeneratorGrid2D<TRoom> : IRandomInjectable, ICancellable, IObservableGenerator<LayoutGrid2D<TRoom>>
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
        public RecursiveGraphBasedGeneratorGrid2D(LevelDescriptionGrid2D<TRoom> levelDescription, GraphBasedGeneratorConfiguration<TRoom> configuration = null)
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

            // TODO: fix
            // Dummy room template
            var dummyRoomTemplate =
                new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(140), new SimpleDoorModeGrid2D(1, 20));

            //dummyRoomTemplate =
            //    new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(16), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            //    {
            //        new DoorGrid2D(new Vector2Int(0,7), new Vector2Int(0, 8)),
            //        new DoorGrid2D(new Vector2Int(16,7), new Vector2Int(16, 8)),
            //        new DoorGrid2D(new Vector2Int(7, 0), new Vector2Int(8, 0)),
            //        new DoorGrid2D(new Vector2Int(7, 16), new Vector2Int(8, 16)),
            //    }));

            // Create configuration spaces generator
            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());

            // Preprocess information about room templates
            var geometryData = LevelGeometryData<RoomNode<TRoom>>.CreateBackwardsCompatible(
                levelDescriptionMapped,
                configurationSpacesGenerator.GetRoomTemplateInstances,
                new List<RoomTemplateGrid2D>() { dummyRoomTemplate }
            );

            // Get dummy room template instance
            var dummyRoomTemplateInstance = geometryData.RoomTemplateInstances[dummyRoomTemplate].Single();

            // Create generator planner
            var generatorPlanner = new GeneratorPlanner<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>>(configuration.SimulatedAnnealingMaxBranching);
            var isGraphDirected = geometryData.RoomTemplateInstances
                .Values
                .SelectMany(x => x)
                .SelectMany(x => x.DoorLines)
                .Any(x => x.Type != DoorType.Undirected);
            var configurationSpaces = new ConfigurationSpacesGrid2D<ConfigurationGrid2D<TRoom, EnergyData>, RoomNode<TRoom>>(levelDescriptionMapped, null, isGraphDirected);
            var layoutConverter =
                new BasicLayoutConverterGrid2D<TRoom,
                    ConfigurationGrid2D<TRoom, EnergyData>>(levelDescription, configurationSpaces,
                    geometryData.RoomTemplateInstanceToPolygonMapping);


            var mapping = levelDescriptionMapped.GetMapping();
            var clusters = levelDescription
                .Clusters
                .Select(x => new Cluster<RoomNode<TRoom>>(x.Select(y => mapping[y]).ToList()))
                .ToList();
            var graph = levelDescriptionMapped.GetGraph();
            foreach (var edge in graph.Edges)
            {
                var cluster1 = clusters.First(x => x.Nodes.Contains(edge.From));
                var cluster2 = clusters.First(x => x.Nodes.Contains(edge.To));

                if (cluster1 != cluster2)
                {
                    cluster1.Edges.Add(new ClusterEdge<RoomNode<TRoom>>(edge.From, edge.To, cluster2));
                    cluster2.Edges.Add(new ClusterEdge<RoomNode<TRoom>>(edge.To, edge.From, cluster1));
                }
            }

            if (clusters.Count > 1 && clusters.Any(x => x.Edges.Count == 0))
            {
                throw new InvalidOperationException();
            }

            var i = 0;
            var chains = clusters.Select(x => new Chain<RoomNode<TRoom>>(x.Nodes, i++, false)).ToList();

            var initialLayout = new Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>(levelDescriptionMapped.GetGraph());
            var layoutEvolver = new RecursiveLayoutEvolver<TRoom>(levelDescription, levelDescriptionMapped, geometryData, clusters, dummyRoomTemplateInstance);


            // Create the generator itself
            generator = new ChainBasedGenerator<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, LayoutGrid2D<TRoom>, RoomNode<TRoom>>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            // Register event handlers
            generator.OnRandomInjected += (random) =>
            {
                //((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
                ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                //((IRandomInjectable)roomShapesHandler).InjectRandomGenerator(random);
            };

            generator.OnCancellationTokenInjected += (token) =>
            {
                ((ICancellable)generatorPlanner).SetCancellationToken(token);
                ((ICancellable)layoutEvolver).SetCancellationToken(token);
            };

            // layoutEvolver.OnEvent += (sender, args) => OnSimulatedAnnealingEvent?.Invoke(sender, args);
            layoutEvolver.OnPerturbed += (sender, layout) => OnPerturbed?.Invoke(layoutConverter.Convert(layout, false));
            layoutEvolver.OnPerturbed += (sender, layout) => OnPerturbedInternal?.Invoke(layout);
            layoutEvolver.OnValid += (sender, layout) => OnPartialValid?.Invoke(layoutConverter.Convert(layout, true));
            generatorPlanner.OnLayoutGenerated += layout => OnValid?.Invoke(layoutConverter.Convert(layout, true));
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

                // TODO: improve
                if (configuration.EarlyStopIfTimeExceeded.HasValue && /*iterations % 100 == 0 &&*/ DateTime.Now - generatorStarted > configuration.EarlyStopIfTimeExceeded)
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