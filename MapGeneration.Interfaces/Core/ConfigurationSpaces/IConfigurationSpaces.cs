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
		/// <summary>
		/// Gets a random point in the maximum intersection of configuration spaces
		/// of given configurations with respect to the main configuration.
		/// </summary>
		/// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
		/// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
		/// <returns></returns>
		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		/// <summary>
		/// Gets a random point in the maximum intersection of configuration spaces
		/// of given configurations with respect to the main configuration.
		/// </summary>
		/// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
		/// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
		/// <param name="configurationsSatisfied">How many of given configurations were satisfied.</param>
		/// <returns></returns>
		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied);

		/// <summary>
		/// Gets a maximum intersection of configuration spaces of given
		/// configurations with respect to the main configuration.
		/// </summary>
		/// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
		/// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
		/// <returns></returns>
		IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		/// <summary>
		/// Gets a maximum intersection of configuration spaces of given
		/// configurations with respect to the main configuration.
		/// </summary>
		/// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
		/// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
		/// <param name="configurationsSatisfied">How many of given configurations were satisfied.</param>
		/// <returns></returns>
		IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied);

		/// <summary>
		/// Gets a random shape for a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		TShape GetRandomShape(TNode node);

		/// <summary>
		/// Checks whether a node can be shape perturbed.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		bool CanPerturbShape(TNode node);

		/// <summary>
		/// Gets all possible shapes for a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		IReadOnlyCollection<TShape> GetShapesForNode(TNode node);

		/// <summary>
		/// Gets all shapes that can be used for at least one node.
		/// </summary>
		/// <returns></returns>
		IEnumerable<TShape> GetAllShapes();

		/// <summary>
		/// Checks if two configurations are valid with respect to each other.
		/// </summary>
		/// <param name="configuration1"></param>
		/// <param name="configuration2"></param>
		/// <returns></returns>
		bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);

		// TODO: remove later
        /// <summary>
        /// Gets a configuration space for two shapes.
        /// </summary>
        /// <param name="movingPolygon">A polygon that can be moved through the returned configuration space.</param>
        /// <param name="fixedPolygon">A polygon that stays fixed.</param>
        /// <returns></returns>
        TConfigurationSpace GetConfigurationSpace(TShape movingPolygon, TShape fixedPolygon);

        /// <summary>
        /// Gets a configuration space for two configurations.
        /// </summary>
        /// <returns></returns>
        TConfigurationSpace GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2);
	}
}