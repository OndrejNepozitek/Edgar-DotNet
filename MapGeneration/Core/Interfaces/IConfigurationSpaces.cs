namespace MapGeneration.Core.Interfaces
{
	using System;
	using System.Collections.Generic;
	using ConfigSpaces;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IConfigurationSpaces<in TNode> : IRandomInjectable
	{
		IntVector2 GetRandomIntersection(IList<Configuration> configurations, Configuration mainConfiguration);

		IList<IntVector2> GetMaximumIntersection(List<Configuration> configurations, Configuration mainConfiguration);

		GridPolygon GetRandomShape();

		GridPolygon GetRandomShape(TNode node);

		bool CanPerturbShape(TNode node);

		IReadOnlyCollection<GridPolygon> GetAllShapes();

		bool HaveValidPosition(Configuration configuration1, Configuration configuration2);
	}
}