namespace MapGeneration.Utils
{
	using Core;
	using Core.Configuration;
	using Core.Configuration.EnergyData;
	using Core.LayoutOperations;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Polygons;

	public static class LayoutGeneratorFactory
	{
		public static SALayoutGenerator<Layout<Configuration<EnergyData>>, TNode, Configuration<EnergyData>> GetDefaultSALayoutGenerator<TNode>()
		{
			return new SALayoutGenerator<Layout<Configuration<EnergyData>>, TNode, Configuration<EnergyData>>(
				graph => new Layout<Configuration<EnergyData>>(graph),
				(configurationSpaces, sigma) => new LayoutOperationsWithConstraints<Layout<Configuration<EnergyData>>, int, Configuration<EnergyData>, IntAlias<GridPolygon>, EnergyData>(configurationSpaces, new PolygonOverlap(), sigma)
			);
		}
	}
}