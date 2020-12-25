using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.ConfigurationSpaces;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;
using ConfigurationSpacesGenerator = Edgar.GraphBasedGenerator.Grid2D.Internal.ConfigurationSpacesGenerator;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
    public class LazyConfigurationSpaces : IConfigurationSpaces<LazyConfigurationSpaces.Configuration, Vector2Int>, IRandomInjectable
    {
        private Random random;
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;
        protected Dictionary<ConfigurationSpaceSelector, ConfigurationSpaceGrid2D> ConfigurationSpaces = new Dictionary<ConfigurationSpaceSelector, ConfigurationSpaceGrid2D>();
        private readonly OrthogonalLineIntersection lineIntersection;

        public LazyConfigurationSpaces(ConfigurationSpacesGenerator configurationSpacesGenerator, OrthogonalLineIntersection lineIntersection)
        {
            this.configurationSpacesGenerator = configurationSpacesGenerator;
            this.lineIntersection = lineIntersection;
        }

        public bool HaveValidPosition(Configuration configuration1, Configuration configuration2)
        {
            var space = GetConfigurationSpace(configuration1, configuration2);
            var lines1 = new List<OrthogonalLineGrid2D>() { new OrthogonalLineGrid2D(configuration1.Position, configuration1.Position) };

            return lineIntersection.DoIntersect(space.Lines.Select(x => FastAddition(x, configuration2.Position)), lines1);
        }
        private OrthogonalLineGrid2D FastAddition(OrthogonalLineGrid2D line, Vector2Int position)
        {
            return new OrthogonalLineGrid2D(line.From + position, line.To + position);
        }

        public ConfigurationSpaceGrid2D GetConfigurationSpace(Configuration movingRoomConfiguration, Configuration fixedRoomConfiguration)
        {
            if (fixedRoomConfiguration.ConnectWithCorridors == null ||
                fixedRoomConfiguration.ConnectWithCorridors.Count == 0)
            {
                return GetConfigurationSpace(movingRoomConfiguration.RoomTemplateInstance,
                    fixedRoomConfiguration.RoomTemplateInstance, null);
            }

            var configurationSpaceLines = new List<OrthogonalLineGrid2D>();

            foreach (var corridor in fixedRoomConfiguration.ConnectWithCorridors)
            {
                var corridorConfigurationSpace = GetConfigurationSpace(movingRoomConfiguration.RoomTemplateInstance,
                    fixedRoomConfiguration.RoomTemplateInstance, corridor);

                configurationSpaceLines.AddRange(corridorConfigurationSpace.Lines);
            }

            configurationSpaceLines = lineIntersection.RemoveIntersections(configurationSpaceLines);

            return new ConfigurationSpaceGrid2D(configurationSpaceLines);
        }

        protected List<Tuple<Configuration, ConfigurationSpaceGrid2D>> GetConfigurationSpaces(Configuration movingRoomConfiguration, List<Configuration> fixedRoomConfigurations)
        {
            var spaces = new List<Tuple<Configuration, ConfigurationSpaceGrid2D>>();

            foreach (var otherConfiguration in fixedRoomConfigurations)
            {
                // TODO: is is not optimal to call the GetConfigurationSpace method over and over again
                spaces.Add(Tuple.Create(otherConfiguration, GetConfigurationSpace(movingRoomConfiguration, otherConfiguration)));
            }

            return spaces;
        }

        IConfigurationSpace<Vector2Int> IConfigurationSpaces<Configuration, Vector2Int>.GetConfigurationSpace(Configuration configuration1, Configuration configuration2)
        {
            return GetConfigurationSpace(configuration1, configuration2);
        }

        public IConfigurationSpace<Vector2Int> GetMaximumIntersection(Configuration mainConfiguration, IEnumerable<Configuration> configurations)
        {
            throw new NotImplementedException();
        }

        public IConfigurationSpace<Vector2Int> GetMaximumIntersection(Configuration mainConfiguration, IEnumerable<Configuration> configurations, out int configurationsSatisfied)
        {
            return GetMaximumIntersection(mainConfiguration, configurations, 0, out configurationsSatisfied);
        }

        public ConfigurationSpaceGrid2D GetMaximumIntersection(Configuration mainConfiguration, IEnumerable<Configuration> configurations, int minimumSatisfiedConfigurations, out int configurationsSatisfied)
        {
            var configurationsSpaces = GetConfigurationSpaces(mainConfiguration, configurations.ToList());
            var intersection = GetMaximumIntersection(configurationsSpaces, minimumSatisfiedConfigurations,
                out configurationsSatisfied);

            return intersection;
        }

        private ConfigurationSpaceGrid2D GetMaximumIntersection(List<Tuple<Configuration, ConfigurationSpaceGrid2D>> configurationSpaces, int minimumSatisfiedConfigurations, out int configurationsSatisfied)
        {
            var spaces = configurationSpaces;
            spaces.Shuffle(random);

            for (var i = spaces.Count; i > 0; i--)
            {
                if (i < minimumSatisfiedConfigurations)
                {
                    break;
                }

                foreach (var indices in spaces.GetCombinations(i))
                {
                    List<OrthogonalLineGrid2D> intersection = null;

                    foreach (var index in indices)
                    {
                        var linesToIntersect = spaces[index].Item2.Lines.Select(x => x + spaces[index].Item1.Position).ToList();
                        intersection = intersection != null
                            ? lineIntersection.GetIntersections(linesToIntersect, intersection)
                            : linesToIntersect;

                        if (intersection.Count == 0)
                        {
                            break;
                        }
                    }

                    if (intersection != null && intersection.Count != 0)
                    {
                        configurationsSatisfied = indices.Length;

                        return new ConfigurationSpaceGrid2D(intersection);
                    }
                }
            }

            configurationsSatisfied = 0;
            return null;
        }

        private ConfigurationSpaceGrid2D GetConfigurationSpace(RoomTemplateInstanceGrid2D movingRoomTemplate, RoomTemplateInstanceGrid2D fixedRoomTemplate, RoomTemplateInstanceGrid2D corridorRoomTemplate)
        {
            var selector = new ConfigurationSpaceSelector(
                movingRoomTemplate,
                fixedRoomTemplate,
                corridorRoomTemplate
            );

            // Check if we already have a configuration space computed for the selector
            if (!ConfigurationSpaces.TryGetValue(selector, out var configurationSpace))
            {
                if (corridorRoomTemplate != null)
                {
                    // TODO: is this the proper way?
                    var configurationSpaceOld = configurationSpacesGenerator.GetConfigurationSpaceOverCorridors(
                        movingRoomTemplate, fixedRoomTemplate, new List<RoomTemplateInstanceGrid2D>() { corridorRoomTemplate });
                    configurationSpace = new ConfigurationSpaceGrid2D(configurationSpaceOld.Lines, null);

                    ConfigurationSpaces[selector] = configurationSpace;
                }
                else
                {
                    configurationSpace = configurationSpacesGenerator.GetConfigurationSpace(
                        movingRoomTemplate, fixedRoomTemplate);
                    ConfigurationSpaces[selector] = configurationSpace;
                }
            }

            return configurationSpace;
        }

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }

        public class Configuration
        {
            public Vector2Int Position { get; }

            public RoomTemplateInstanceGrid2D RoomTemplateInstance { get; }

            public List<RoomTemplateInstanceGrid2D> ConnectWithCorridors { get; }

            public Configuration(Vector2Int position, RoomTemplateInstanceGrid2D roomTemplateInstance, List<RoomTemplateInstanceGrid2D> connectWithCorridors)
            {
                Position = position;
                RoomTemplateInstance = roomTemplateInstance;
                ConnectWithCorridors = connectWithCorridors;
            }
        }
    }
}