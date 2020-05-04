using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Simplified.ConfigurationSpaces;

namespace MapGeneration.Core.ConfigurationSpaces.Interfaces
{
    /// <summary>
	/// Represents a data structure holding configuration spaces.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TShape"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
	/// <typeparam name="TConfigurationSpace"></typeparam>
	public interface ILegacyConfigurationSpaces<in TNode, TShape, TConfiguration, out TConfigurationSpace> : IConfigurationSpaces<TConfiguration, TConfigurationSpace>
	{
        /// <summary>
        /// Gets a maximum intersection of configuration spaces of given
        /// configurations with respect to the main configuration.
        /// </summary>
        /// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
        /// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
        /// <returns></returns>
        List<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, List<TConfiguration> configurations);

        /// <summary>
        /// Gets a maximum intersection of configuration spaces of given
        /// configurations with respect to the main configuration.
        /// </summary>
        /// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
        /// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
        /// <param name="configurationsSatisfied">How many of given configurations were satisfied.</param>
        /// <returns></returns>
        List<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, List<TConfiguration> configurations, out int configurationsSatisfied);

		/// <summary>
		/// Gets a random point in the maximum intersection of configuration spaces
		/// of given configurations with respect to the main configuration.
		/// </summary>
		/// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
		/// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
		/// <returns></returns>
		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, List<TConfiguration> configurations);

		/// <summary>
		/// Gets a random point in the maximum intersection of configuration spaces
		/// of given configurations with respect to the main configuration.
		/// </summary>
		/// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
		/// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
		/// <param name="configurationsSatisfied">How many of given configurations were satisfied.</param>
		/// <returns></returns>
		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, List<TConfiguration> configurations, out int configurationsSatisfied);

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

        // TODO: remove later
        /// <summary>
        /// Gets a configuration space for two shapes.
        /// </summary>
        /// <param name="movingPolygon">A polygon that can be moved through the returned configuration space.</param>
        /// <param name="fixedPolygon">A polygon that stays fixed.</param>
        /// <returns></returns>
        TConfigurationSpace GetConfigurationSpace(TShape movingPolygon, TShape fixedPolygon);
    }
}