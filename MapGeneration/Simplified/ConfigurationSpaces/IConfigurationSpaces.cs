using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;

namespace MapGeneration.Simplified.ConfigurationSpaces
{
    public interface IConfigurationSpaces<TConfiguration, out TConfigurationSpace>
    {
        /// <summary>
        /// Gets a maximum intersection of configuration spaces of given
        /// configurations with respect to the main configuration.
        /// </summary>
        /// <param name="mainConfiguration">Configuration of a node for which we look for a new position.</param>
        /// <param name="configurations">Configurations that we try to satisfy when looking for a new position.</param>
        /// <param name="minimumSatisfiedConfigurations">The minimum number of configurations that need to be satisfied.</param>
        /// <param name="configurationsSatisfied">How many of given configurations were satisfied.</param>
        /// <returns></returns>
        List<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, List<TConfiguration> configurations, int minimumSatisfiedConfigurations, out int configurationsSatisfied);
        
        /// <summary>
        /// Gets a configuration space for two configurations.
        /// </summary>
        /// <returns></returns>
        TConfigurationSpace GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2);

        /// <summary>
        /// Checks if two configurations are valid with respect to each other.
        /// </summary>
        /// <param name="configuration1"></param>
        /// <param name="configuration2"></param>
        /// <returns></returns>
        bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);
    }
}