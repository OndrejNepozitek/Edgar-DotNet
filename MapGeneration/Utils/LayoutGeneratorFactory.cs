using MapGeneration.Interfaces.Core.LayoutGenerator;
using MapGeneration.Interfaces.Utils;

namespace MapGeneration.Utils
{
	using System.Collections.Generic;
	using Core.ChainDecompositions;
	using Core.Configurations;
	using Core.Configurations.EnergyData;
	using Core.ConfigurationSpaces;
	using Core.Constraints;
	using Core.Doors;
	using Core.GeneratorPlanners;
	using Core.LayoutConverters;
	using Core.LayoutEvolvers;
	using Core.LayoutGenerators;
	using Core.LayoutOperations;
	using Core.Layouts;
	using Core.MapDescriptions;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.MapLayouts;

	public static class LayoutGeneratorFactory
	{
		/// <summary>
		/// Gets a basic layout generator that should not be used to generated layouts with corridors.
		/// </summary>
		/// <typeparam name="TNode"></typeparam>
		/// <returns></returns>
		public static ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, IMapLayout<TNode>> GetDefaultChainBasedGenerator<TNode>()
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, IMapLayout<TNode>>();

			var chainDecomposition = new BreadthFirstChainDecomposition<int>();
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlannerOld<Layout<Configuration<EnergyData>, BasicEnergyData>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => chainDecomposition);
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<TNode, Configuration<EnergyData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<EnergyData>, BasicEnergyData>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<EnergyData>, BasicEnergyData>, TNode, Configuration<EnergyData>>(mapDescription, configurationSpaces, configurationSpacesGenerator.LastIntAliasMapping));
			layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>(layoutOperations));
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var layoutOperations = new LayoutOperationsWithConstraints<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, IntAlias<GridPolygon>, EnergyData, BasicEnergyData>(configurationSpaces, configurationSpaces.GetAverageSize());

				var averageSize = configurationSpaces.GetAverageSize();

				layoutOperations.AddNodeConstraint(new BasicContraint<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, EnergyData, IntAlias<GridPolygon>>(
					new FastPolygonOverlap(), 
					averageSize,
					configurationSpaces
				));

				return layoutOperations;
			});

			return layoutGenerator;
		}

        /// <summary>
        /// Gets a basic layout generator that should not be used to generated layouts with corridors.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <returns></returns>
        public static SimpleChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<EnergyData>, BasicEnergyData>, IMapLayout<TNode>, int> GetSimpleChainBasedGenerator<TNode>(MapDescription<TNode> mapDescription)
        {
            var chainDecomposition = new BreadthFirstChainDecomposition<int>();
            var chains = chainDecomposition.GetChains(mapDescription.GetGraph());

            var generatorPlanner = new GeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>, int>();

            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
            var configurationSpaces = configurationSpacesGenerator.Generate<TNode, Configuration<EnergyData>>(mapDescription);

            var initialLayout = new Layout<Configuration<EnergyData>, BasicEnergyData>(mapDescription.GetGraph());
            var layoutConverter =
                new BasicLayoutConverter<Layout<Configuration<EnergyData>, BasicEnergyData>, TNode,
                    Configuration<EnergyData>>(mapDescription, configurationSpaces,
                    configurationSpacesGenerator.LastIntAliasMapping);

            var layoutOperations = new LayoutOperationsWithConstraints<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, IntAlias<GridPolygon>, EnergyData, BasicEnergyData>(configurationSpaces, configurationSpaces.GetAverageSize());

            var averageSize = configurationSpaces.GetAverageSize();

            layoutOperations.AddNodeConstraint(new BasicContraint<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, EnergyData, IntAlias<GridPolygon>>(
                new FastPolygonOverlap(),
                averageSize,
                configurationSpaces
            ));

            var layoutEvolver =
                new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
                    Configuration<EnergyData>>(layoutOperations, true);

            var layoutGenerator = new SimpleChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<EnergyData>, BasicEnergyData>, IMapLayout<TNode>, int>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            layoutGenerator.OnRandomInjected += (random) =>
            {
                ((IRandomInjectable) configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable) layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable) layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable) layoutConverter).InjectRandomGenerator(random);
            };

            layoutGenerator.OnCancellationTokenInjected += (token) =>
            {
                ((ICancellable) generatorPlanner).SetCancellationToken(token);
                ((ICancellable) layoutEvolver).SetCancellationToken(token);
            };

            return layoutGenerator;
        }

        /// <summary>
        /// Gets a generator that can work with corridors.
        /// </summary>
        /// <param name="offsets"></param>
        /// <param name="canTouch">Whether rooms can touch. Perfomance is decreased when set to false.</param>
        /// <returns></returns>
        public static ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>, IMapLayout<TNode>> GetChainBasedGeneratorWithCorridors<TNode>(List<int> offsets, bool canTouch = false)
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>, IMapLayout<TNode>>();

			var chainDecomposition = new BreadthFirstChainDecomposition<int>();
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlannerOld<Layout<Configuration<CorridorsData>, BasicEnergyData>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => new CorridorsChainDecomposition<int>(mapDescription, chainDecomposition));
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<TNode, Configuration<CorridorsData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<CorridorsData>, BasicEnergyData>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<CorridorsData>, BasicEnergyData>, TNode, Configuration<CorridorsData>>(mapDescription, configurationSpaces, configurationSpacesGenerator.LastIntAliasMapping));
			layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => new SimulatedAnnealingEvolver<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>>(layoutOperations));
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var corridorConfigurationSpaces = configurationSpacesGenerator.Generate<TNode, Configuration<CorridorsData>>(mapDescription, offsets);
				var layoutOperations = new LayoutOperationsWithCorridors<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>, IntAlias<GridPolygon>, CorridorsData, BasicEnergyData>(configurationSpaces, mapDescription, corridorConfigurationSpaces, configurationSpaces.GetAverageSize());
				var polygonOverlap = new FastPolygonOverlap();

				var averageSize = configurationSpaces.GetAverageSize();

				layoutOperations.AddNodeConstraint(new BasicContraint<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
					polygonOverlap,
					averageSize,
					configurationSpaces
				));

				layoutOperations.AddNodeConstraint(new CorridorConstraints<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
					mapDescription,
					averageSize,
					corridorConfigurationSpaces
				));

				if (!canTouch)
				{
					layoutOperations.AddNodeConstraint(new TouchingConstraints<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
						mapDescription,
						polygonOverlap
					));
				}

				return layoutOperations;
			});

			return layoutGenerator;
		}
	}
}