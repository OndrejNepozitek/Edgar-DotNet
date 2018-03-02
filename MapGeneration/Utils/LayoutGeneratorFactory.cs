namespace MapGeneration.Utils
{
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Core.Configuration;
	using Core.Configuration.EnergyData;
	using Core.ConfigurationSpaces;
	using Core.Constraints;
	using Core.Doors;
	using Core.GraphDecomposition;
	using Core.LayoutOperations;
	using Core.SimulatedAnnealing.GeneratorPlanner;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Polygons;

	public static class LayoutGeneratorFactory
	{
		public static SALayoutGenerator<MapDescription<int>, Layout<Configuration<EnergyData>>, Configuration<EnergyData>> GetDefaultSALayoutGenerator()
		{
			var layoutGenerator = new SALayoutGenerator<MapDescription<int>, Layout<Configuration<EnergyData>>, Configuration<EnergyData>>();

			var chainDecomposition = new BreadthFirstLongerChainsDecomposition<int>(new GraphDecomposer<int>());
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new LazyGeneratorPlanner<Layout<Configuration<EnergyData>>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => chainDecomposition);
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<int, Configuration<EnergyData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<EnergyData>>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var layoutOperations = new LayoutOperationsWithConstraints<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>, IntAlias<GridPolygon>, EnergyData>(configurationSpaces);

				var averageSize = GetAverageSize(configurationSpaces.GetAllShapes());

				layoutOperations.AddContraints(new BasicContraint<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>, EnergyData, IntAlias<GridPolygon>>(
					new PolygonOverlap(),
					averageSize,
					configurationSpaces
				));

				return layoutOperations;
			});

			return layoutGenerator;
		}

		public static SALayoutGenerator<MapDescription<int>, Layout<Configuration<EnergyDataCorridors>>, Configuration<EnergyDataCorridors>> GetSALayoutGeneratorWithCorridors(List<int> offsets)
		{
			var layoutGenerator = new SALayoutGenerator<MapDescription<int>, Layout<Configuration<EnergyDataCorridors>>, Configuration<EnergyDataCorridors>>();

			var chainDecomposition = new BreadthFirstLongerChainsDecomposition<int>(new GraphDecomposer<int>());
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new LazyGeneratorPlanner<Layout<Configuration<EnergyDataCorridors>>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => new CorridorsChainDecomposition<int>(mapDescription, chainDecomposition));
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<int, Configuration<EnergyDataCorridors>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<EnergyDataCorridors>>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var corridorConfigurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<EnergyDataCorridors>>(mapDescription, offsets);
				var layoutOperations = new LayoutOperationsWithCorridors<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, IntAlias<GridPolygon>, EnergyDataCorridors>(configurationSpaces, mapDescription, corridorConfigurationSpaces);

				var averageSize = GetAverageSize(configurationSpaces.GetAllShapes());

				layoutOperations.AddContraints(new BasicContraint<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, EnergyDataCorridors, IntAlias<GridPolygon>>(
					new PolygonOverlap(), 
					averageSize,
					configurationSpaces
				));

				layoutOperations.AddContraints(new CorridorConstraints<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, EnergyDataCorridors, IntAlias<GridPolygon>>(
					mapDescription,
					averageSize,
					corridorConfigurationSpaces
				));

				return layoutOperations;
			});

			return layoutGenerator;
		}

		// TODO: should it be here?
		private static int GetAverageSize(IEnumerable<IntAlias<GridPolygon>> polygons)
		{
			return (int)polygons.Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);
		}
	}
}