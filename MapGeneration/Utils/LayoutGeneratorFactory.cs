using MapGeneration.Core.Configurations.EnergyData;

namespace MapGeneration.Utils
{
	using System.Collections.Generic;
	using Core.ChainDecompositions;
	using Core.Configurations;
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
		public static ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, IMapLayout<TNode>> GetDefaultChainBasedGenerator<TNode>()
        {
            return GetChainBasedGenerator<TNode>(false);
        }

		/// <summary>
		/// Gets a generator that can work with corridors.
		/// </summary>
		/// <param name="offsets"></param>
		/// <param name="canTouch">Whether rooms can touch. Performance is decreased when set to false.</param>
		/// <returns></returns>
		public static ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, IMapLayout<TNode>> GetChainBasedGenerator<TNode>(bool withCorridors, List<int> offsets = null, bool canTouch = false)
		{
			var layoutGenerator = new ChainBasedGenerator<MapDescription<TNode>, Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, IMapLayout<TNode>>();

			var chainDecomposition = new BreadthFirstChainDecomposition<int>();
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			var generatorPlanner = new BasicGeneratorPlanner<Layout<Configuration<CorridorsData>>>();

			layoutGenerator.SetChainDecompositionCreator(mapDescription => new TwoStageChainDecomposition<int>(mapDescription, chainDecomposition));
			layoutGenerator.SetConfigurationSpacesCreator(mapDescription => configurationSpacesGenerator.Generate<TNode, Configuration<CorridorsData>>(mapDescription));
			layoutGenerator.SetInitialLayoutCreator(mapDescription => new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph()));
			layoutGenerator.SetGeneratorPlannerCreator(mapDescription => generatorPlanner);
			layoutGenerator.SetLayoutConverterCreator((mapDescription, configurationSpaces) => new BasicLayoutConverter<Layout<Configuration<CorridorsData>>, TNode, Configuration<CorridorsData>>(mapDescription, configurationSpaces, configurationSpacesGenerator.LastIntAliasMapping));
			layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => new SimulatedAnnealingEvolver<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>>(layoutOperations));
			layoutGenerator.SetLayoutOperationsCreator((mapDescription, configurationSpaces) =>
			{
				var corridorConfigurationSpaces = withCorridors ? configurationSpacesGenerator.Generate<TNode, Configuration<CorridorsData>>(mapDescription, offsets) : configurationSpaces;

                var layoutOperations = new LayoutOperationsWithConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, IntAlias<GridPolygon>, CorridorsData>(corridorConfigurationSpaces, configurationSpaces.GetAverageSize(), mapDescription, configurationSpaces);
                var polygonOverlap = new FastPolygonOverlap();

				var averageSize = configurationSpaces.GetAverageSize();

				layoutOperations.AddNodeConstraint(new BasicContraint<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
					polygonOverlap,
					averageSize,
					configurationSpaces
				));

                if (withCorridors)
                {
                    layoutOperations.AddNodeConstraint(new CorridorConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                        mapDescription,
                        averageSize,
                        corridorConfigurationSpaces
                    ));

                    if (!canTouch)
                    {
                        layoutOperations.AddNodeConstraint(new TouchingConstraints<Layout<Configuration<CorridorsData>>, int, Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                            mapDescription,
                            polygonOverlap
                        ));
                    }
				}

				return layoutOperations;
			});

			return layoutGenerator;
		}
	}
}