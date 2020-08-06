using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.ConfigurationSpaces
{
    public interface IConfigurationSpaces<in TConfiguration>
    {
        bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);
    }

    public interface IConfigurationSpaces<in TConfiguration, TPosition> : IConfigurationSpaces<TConfiguration>
    {
        IConfigurationSpace<TPosition> GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2);

        IConfigurationSpace<TPosition> GetMaximumIntersection(TConfiguration mainConfiguration, IEnumerable<TConfiguration> configurations);

        IConfigurationSpace<TPosition> GetMaximumIntersection(TConfiguration mainConfiguration, IEnumerable<TConfiguration> configurations, out int configurationsSatisfied);
    }
}