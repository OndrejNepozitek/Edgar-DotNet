using System;
using System.Collections.Generic;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Simplified
{
    public class LazyConfigurationSpaces<TRoom> : AbstractConfigurationSpaces<TRoom, RoomTemplateInstance, SimpleConfiguration<TRoom>>
    {
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;

        protected Dictionary<RoomTemplateInstance, Dictionary<RoomTemplateInstance, ConfigurationSpace>>
            ConfigurationSpaces =
                new Dictionary<RoomTemplateInstance, Dictionary<RoomTemplateInstance, ConfigurationSpace>>();

        public LazyConfigurationSpaces(ILineIntersection<OrthogonalLine> lineIntersection, ConfigurationSpacesGenerator configurationSpacesGenerator) : base(lineIntersection)
        {
            this.configurationSpacesGenerator = configurationSpacesGenerator;
        }

        protected override IList<Tuple<SimpleConfiguration<TRoom>, ConfigurationSpace>> GetConfigurationSpaces(SimpleConfiguration<TRoom> movingRoomConfiguration, IList<SimpleConfiguration<TRoom>> fixedRoomConfigurations)
        {
            var spaces = new List<Tuple<SimpleConfiguration<TRoom>, ConfigurationSpace>>();

            foreach (var otherConfiguration in fixedRoomConfigurations)
            {
                // TODO: is is not optimal to call the GetConfigurationSpace method over and over again
                spaces.Add(Tuple.Create(otherConfiguration, GetConfigurationSpace(movingRoomConfiguration, otherConfiguration)));
            }

            return spaces;
        }

        public override ConfigurationSpace GetConfigurationSpace(SimpleConfiguration<TRoom> movingRoomConfiguration, SimpleConfiguration<TRoom> fixedRoomConfiguration)
        {
            // Check if we already have a dictionary of configuration spaces for the moving room template instance
            // If not, create an empty dictionary
            if (!ConfigurationSpaces.TryGetValue(movingRoomConfiguration.RoomTemplateInstance, out var movingRoomConfigurationSpaces))
            {
                movingRoomConfigurationSpaces = new Dictionary<RoomTemplateInstance, ConfigurationSpace>();
                ConfigurationSpaces[movingRoomConfiguration.RoomTemplateInstance] = movingRoomConfigurationSpaces;
            }

            // Check if we already TODO
            if (!movingRoomConfigurationSpaces.TryGetValue(fixedRoomConfiguration.RoomTemplateInstance, out var configurationSpace))
            {
                configurationSpace = configurationSpacesGenerator.GetConfigurationSpace(
                    movingRoomConfiguration.RoomTemplateInstance, fixedRoomConfiguration.RoomTemplateInstance);
                movingRoomConfigurationSpaces[fixedRoomConfiguration.RoomTemplateInstance] = configurationSpace;
            }

            return configurationSpace;
        }

        public override ConfigurationSpace GetConfigurationSpace(RoomTemplateInstance movingPolygon, RoomTemplateInstance fixedPolygon)
        {
            throw new NotSupportedException();
        }

        public override RoomTemplateInstance GetRandomShape(TRoom node)
        {
            throw new NotSupportedException();
        }

        public override bool CanPerturbShape(TRoom node)
        {
            throw new NotSupportedException();
        }

        public override IReadOnlyCollection<RoomTemplateInstance> GetShapesForNode(TRoom node)
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<RoomTemplateInstance> GetAllShapes()
        {
            throw new NotSupportedException();
        }
    }
}