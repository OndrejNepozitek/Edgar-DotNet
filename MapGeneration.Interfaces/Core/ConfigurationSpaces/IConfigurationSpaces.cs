namespace MapGeneration.Interfaces.Core.ConfigurationSpaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

	/// <summary>
	/// Represents a data structure holding configuration spaces.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TShape"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
	/// <typeparam name="TConfigurationSpace"></typeparam>
	public interface IConfigurationSpaces<in TNode, TShape, TConfiguration, out TConfigurationSpace>
	{
		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied);

		IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied);

		TShape GetRandomShape(TNode node);

		bool CanPerturbShape(TNode node);

		IReadOnlyCollection<TShape> GetShapesForNode(TNode node);

		IEnumerable<TShape> GetAllShapes();

		bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);

		TConfigurationSpace GetConfigurationSpace(TShape shape1, TShape shape2);
	}
}