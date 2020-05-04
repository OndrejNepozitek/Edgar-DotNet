using System;
using System.Collections.Generic;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Simplified.ConfigurationSpaces
{
    public class LazyConfigurationSpaces : ConfigurationSpacesBase<LazyConfigurationSpaces.Configuration>
    {
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;

        protected Dictionary<ConfigurationSpaceSelector, ConfigurationSpace> ConfigurationSpaces = new Dictionary<ConfigurationSpaceSelector, ConfigurationSpace>();

        public LazyConfigurationSpaces(ILineIntersection<OrthogonalLine> lineIntersection, ConfigurationSpacesGenerator configurationSpacesGenerator) : base(lineIntersection)
        {
            this.configurationSpacesGenerator = configurationSpacesGenerator;
        }

        protected List<Tuple<Configuration, ConfigurationSpace>> GetConfigurationSpaces(Configuration movingRoomConfiguration, List<Configuration> fixedRoomConfigurations)
        {
            var spaces = new List<Tuple<Configuration, ConfigurationSpace>>();

            foreach (var otherConfiguration in fixedRoomConfigurations)
            {
                // TODO: is is not optimal to call the GetConfigurationSpace method over and over again
                spaces.Add(Tuple.Create(otherConfiguration, GetConfigurationSpace(movingRoomConfiguration, otherConfiguration)));
            }

            return spaces;
        }

        public override List<OrthogonalLine> GetMaximumIntersection(Configuration mainConfiguration, List<Configuration> configurations,
            int minimumSatisfiedConfigurations, out int configurationsSatisfied)
        {
            var configurationsSpaces = GetConfigurationSpaces(mainConfiguration, configurations);
            var intersection = GetMaximumIntersection(configurationsSpaces, minimumSatisfiedConfigurations,
                out configurationsSatisfied);

            return intersection;
        }

        public override ConfigurationSpace GetConfigurationSpace(Configuration movingRoomConfiguration, Configuration fixedRoomConfiguration)
        {
            if (fixedRoomConfiguration.ConnectWithCorridors == null ||
                fixedRoomConfiguration.ConnectWithCorridors.Count == 0)
            {
                return GetConfigurationSpace(movingRoomConfiguration.RoomTemplateInstance,
                    fixedRoomConfiguration.RoomTemplateInstance, null);
            }

            var configurationSpaceLines = new List<OrthogonalLine>();

            foreach (var corridor in fixedRoomConfiguration.ConnectWithCorridors)
            {
                var corridorConfigurationSpace = GetConfigurationSpace(movingRoomConfiguration.RoomTemplateInstance,
                    fixedRoomConfiguration.RoomTemplateInstance, corridor);

                configurationSpaceLines.AddRange(corridorConfigurationSpace.Lines);
            }

            configurationSpaceLines = LineIntersection.RemoveIntersections(configurationSpaceLines);

            return new ConfigurationSpace()
            {
                Lines = configurationSpaceLines,
            };
        }

        private ConfigurationSpace GetConfigurationSpace(RoomTemplateInstance movingRoomTemplate, RoomTemplateInstance fixedRoomTemplate, RoomTemplateInstance corridorRoomTemplate)
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
                    configurationSpace = configurationSpacesGenerator.GetConfigurationSpaceOverCorridor(
                        movingRoomTemplate, fixedRoomTemplate, corridorRoomTemplate);
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

        public class Configuration : IPositionConfiguration, IShapeConfiguration<RoomTemplateInstance>
        {
            public IntVector2 Position { get; }

            public RoomTemplateInstance RoomTemplateInstance { get; }

            public List<RoomTemplateInstance> ConnectWithCorridors { get; }

            // TODO: weird
            public RoomTemplateInstance ShapeContainer => RoomTemplateInstance;

            public Configuration(IntVector2 position, RoomTemplateInstance roomTemplateInstance, List<RoomTemplateInstance> connectWithCorridors)
            {
                Position = position;
                RoomTemplateInstance = roomTemplateInstance;
                ConnectWithCorridors = connectWithCorridors;
            }
        }
    }
}