namespace MapGeneration.Utils
{
	using System.Collections.Generic;
	using Core;
	using Core.ChainDecompositions;
	using Core.Configurations;
	using Core.Configurations.EnergyData;
	using Core.ConfigurationSpaces;
	using Core.Constraints;
	using Core.Doors;
	using Core.Experimental;
	using Core.GeneratorPlanners;
	using Core.LayoutConverters;
	using Core.LayoutEvolvers;
	using Core.LayoutGenerators;
	using Core.LayoutOperations;
	using Core.Layouts;
	using Core.MapDescriptions;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Configuration.EnergyData;

	public static class LayoutGeneratorFactory
	{
		public static ChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>> GetDefaultChainBasedGenerator()
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>();

			var chainDecomposition = new BreadthFirstChainDecomposition<int>(new GraphDecomposer<int>());
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => chainDecomposition);
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<int, Configuration<EnergyData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<EnergyData>, BasicEnergyData>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>(mapDescription, configurationSpaces));
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
		/// Gets a generator that can work with corridors.
		/// </summary>
		/// <param name="offsets"></param>
		/// <param name="canTouch">Whether rooms can touch. Perfomance is decreased when set to false.</param>
		/// <returns></returns>
		public static ChainBasedGenerator<MapDescription<int>, Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>> GetChainBasedGeneratorWithCorridors(List<int> offsets, bool canTouch = false)
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<int>, Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>>();

			var chainDecomposition = new BreadthFirstChainDecomposition<int>(new GraphDecomposer<int>());
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlanner<Layout<Configuration<CorridorsData>, BasicEnergyData>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => new CorridorsChainDecomposition<int>(mapDescription, chainDecomposition));
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<int, Configuration<CorridorsData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<CorridorsData>, BasicEnergyData>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>>(mapDescription, configurationSpaces));
			layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => new SimulatedAnnealingEvolver<Layout<Configuration<CorridorsData>, BasicEnergyData>, int, Configuration<CorridorsData>>(layoutOperations));
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var corridorConfigurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<CorridorsData>>(mapDescription, offsets);
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