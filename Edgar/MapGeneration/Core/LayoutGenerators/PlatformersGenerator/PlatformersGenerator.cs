using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.Configurations;
using MapGeneration.Core.Configurations.EnergyData;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.Constraints;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.GeneratorPlanners;
using MapGeneration.Core.LayoutConverters;
using MapGeneration.Core.LayoutEvolvers.PlatformersEvolver;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.LayoutGenerators.Interfaces;
using MapGeneration.Core.LayoutOperations;
using MapGeneration.Core.Layouts;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Core.LayoutGenerators.PlatformersGenerator
{
    /// <summary>
    /// This generator is currently not used as the dungeon generator can also handle platformers.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class PlatformersGenerator<TNode> : IRandomInjectable, ICancellable, IObservableGenerator<MapLayout<TNode>>
    {
        private readonly MapDescriptionMapping<TNode> mapDescription;
        private readonly IMapDescription<TNode> mapDescriptionOriginal;
        private readonly DungeonGeneratorConfiguration<TNode> configuration;
        private ChainBasedGenerator<IMapDescription<int>, Layout<Configuration<CorridorsData>>, MapLayout<TNode>, int> generator;

        public event EventHandler<SimulatedAnnealingEventArgs> OnSimulatedAnnealingEvent;

        // Exists because OnPerturbed converts layouts which uses the Random instance and causes results to be different.
        private event Action<Layout<Configuration<CorridorsData>>> OnPerturbedInternal;

        public PlatformersGenerator(IMapDescription<TNode> mapDescription, DungeonGeneratorConfiguration<TNode> configuration = null)
        {
            this.mapDescription = new MapDescriptionMapping<TNode>(mapDescription);
            this.configuration = configuration ?? new DungeonGeneratorConfiguration<TNode>();
            this.mapDescriptionOriginal = mapDescription;
            SetupGenerator();
        }

        // TODO: remove
        public double TimeTotal => generator.TimeTotal;

        public int IterationsCount => generator.IterationsCount;

        private void SetupGenerator()
        {
            var mapping = mapDescription.GetMapping();
            var chainsGeneric = configuration.Chains;

            if (chainsGeneric == null)
            {
                var chainDecomposition = new TwoStageChainDecomposition<TNode>(mapDescriptionOriginal, new BreadthFirstChainDecomposition<TNode>(configuration.ChainDecompositionConfiguration ?? new ChainDecompositionConfiguration()));
                chainsGeneric = chainDecomposition.GetChains(mapDescriptionOriginal.GetGraph());
            }

            var chains = chainsGeneric
                .Select(x => new Chain<int>(x.Nodes.Select(y => mapping[y]).ToList(), x.Number))
                .ToList();

            var generatorPlanner = new GeneratorPlanner<Layout<Configuration<CorridorsData>>, int>(configuration.SimulatedAnnealingMaxBranching);

            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
            var configurationSpaces = configurationSpacesGenerator.GetConfigurationSpaces<Configuration<CorridorsData>>(mapDescription);

            //var corridorConfigurationSpaces = mapDescription.IsWithCorridors ? configurationSpacesGenerator.Generate<TNode, Configuration<CorridorsData>>(mapDescription, mapDescription.CorridorsOffsets) : configurationSpaces;
            var corridorConfigurationSpaces = configurationSpaces;

            var averageSize = configurationSpaces.GetAverageSize();
            var polygonOverlap = new FastPolygonOverlap();

            var stageOneConstraints =
                new List<INodeConstraint<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>,
                    CorridorsData>>
                {
                    new BasicConstraint<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>,
                        CorridorsData, IntAlias<GridPolygon>>(
                        new FastPolygonOverlap(),
                        averageSize,
                        configurationSpaces
                    ),
                    new CorridorConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                            mapDescription,
                            averageSize,
                            corridorConfigurationSpaces
                        ),
                };

            if (!configuration.RoomsCanTouch)
            {
                stageOneConstraints.Add(new TouchingConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                    mapDescription,
                    polygonOverlap
                ));
            }

            var stageOneConstraintsEvaluator = new ConstraintsEvaluator<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, IntAlias<GridPolygon>, CorridorsData>(stageOneConstraints);

            //if (mapDescription.IsWithCorridors)
            //{
            //    layoutOperations.AddNodeConstraint(new CorridorConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
            //        mapDescription,
            //        averageSize,
            //        corridorConfigurationSpaces
            //    ));

            //    if (!false) // TODO:
            //    {
            //        var polygonOverlap = new FastPolygonOverlap();
            //        layoutOperations.AddNodeConstraint(new TouchingConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
            //            mapDescription,
            //            polygonOverlap
            //        ));
            //    }
            //}


            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription,
                configuration.RepeatModeOverride
            );

            var layoutOperations = new LayoutOperations<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, IntAlias<GridPolygon>, CorridorsData>(corridorConfigurationSpaces, configurationSpaces.GetAverageSize(), mapDescription, stageOneConstraintsEvaluator, stageOneConstraintsEvaluator, roomShapesHandler, configuration.ThrowIfRepeatModeNotSatisfied);

            var initialLayout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            var layoutConverter =
                new BasicLayoutConverter<Layout<Configuration<CorridorsData>>, TNode,
                    Configuration<CorridorsData>>(mapDescription, configurationSpaces,
                    configurationSpaces.GetIntAliasMapping());

            var layoutEvolver =
                    new PlatformersEvolver<Layout<Configuration<CorridorsData>>, int,
                        Configuration<CorridorsData>>(layoutOperations);

            generator = new ChainBasedGenerator<IMapDescription<int>, Layout<Configuration<CorridorsData>>, MapLayout<TNode>, int>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            generator.OnRandomInjected += (random) =>
            {
                ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
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

        public MapLayout<TNode> GenerateLayout()
        {
            var earlyStoppingHandler = GetEarlyStoppingHandler(DateTime.Now);

            OnPerturbedInternal += earlyStoppingHandler;
            var layout = generator.GenerateLayout();
            OnPerturbedInternal -= earlyStoppingHandler;

            return layout;
        }

        private Action<Layout<Configuration<CorridorsData>>> GetEarlyStoppingHandler(DateTime generatorStarted)
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