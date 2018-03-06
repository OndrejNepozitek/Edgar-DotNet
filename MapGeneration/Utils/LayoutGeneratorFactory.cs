namespace MapGeneration.Utils
{
	using System.Collections.Generic;
	using Core;
	using Core.Configurations;
	using Core.Configurations.EnergyData;
	using Core.ConfigurationSpaces;
	using Core.Constraints;
	using Core.Doors;
	using Core.Experimental;
	using Core.GeneratorPlanners;
	using Core.GraphDecomposition;
	using Core.LayoutConverters;
	using Core.LayoutEvolvers;
	using Core.LayoutGenerators;
	using Core.LayoutOperations;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public static class LayoutGeneratorFactory
	{
		public static ChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>> GetDefaultSALayoutGenerator()
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>>();

			var chainDecomposition = new BreadthFirstLongerChainsDecomposition<int>(new GraphDecomposer<int>());
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlanner<Layout<Configuration<EnergyData>>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => chainDecomposition);
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<int, Configuration<EnergyData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<EnergyData>>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>>(mapDescription, configurationSpaces));
			layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>>(layoutOperations));
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var layoutOperations = new LayoutOperationsWithConstraints<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>, IntAlias<GridPolygon>, EnergyData>(configurationSpaces, configurationSpaces.GetAverageSize());

				var averageSize = configurationSpaces.GetAverageSize();

				layoutOperations.AddContraints(new BasicContraint<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>, EnergyData, IntAlias<GridPolygon>>(
					new FastPolygonOverlap(), 
					averageSize,
					configurationSpaces
				));

				return layoutOperations;
			});

			return layoutGenerator;
		}

		public static ChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>> GetSALayoutGeneratorWithCorridors(List<int> offsets)
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>>();

			var chainDecomposition = new BreadthFirstLongerChainsDecomposition<int>(new GraphDecomposer<int>());
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlanner<Layout<Configuration<EnergyDataCorridors>>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => new CorridorsChainDecomposition<int>(mapDescription, chainDecomposition));
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<int, Configuration<EnergyDataCorridors>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<EnergyDataCorridors>>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>>(mapDescription, configurationSpaces));
			layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => new SimulatedAnnealingEvolver<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>>(layoutOperations));
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var corridorConfigurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<EnergyDataCorridors>>(mapDescription, offsets);
				var layoutOperations = new LayoutOperationsWithCorridors<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, IntAlias<GridPolygon>, EnergyDataCorridors>(configurationSpaces, mapDescription, corridorConfigurationSpaces, configurationSpaces.GetAverageSize());
				var polygonOverlap = new FastPolygonOverlap();

				var averageSize = configurationSpaces.GetAverageSize();

				layoutOperations.AddContraints(new BasicContraint<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, EnergyDataCorridors, IntAlias<GridPolygon>>(
					polygonOverlap,
					averageSize,
					configurationSpaces
				));

				layoutOperations.AddContraints(new CorridorConstraints<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, EnergyDataCorridors, IntAlias<GridPolygon>>(
					mapDescription,
					averageSize,
					corridorConfigurationSpaces
				));

				layoutOperations.AddContraints(new TouchingConstraints<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, EnergyDataCorridors, IntAlias<GridPolygon>>(
					mapDescription,
					polygonOverlap
				));

				return layoutOperations;
			});

			return layoutGenerator;
		}
	}
}