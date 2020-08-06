using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Configurations;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.ConfigurationSpaces.Interfaces;
using MapGeneration.Core.LayoutOperations.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator
{
    /// <summary>
    /// Class responsible for returning available shapes for a node based on used repeat mode.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TConfiguration"></typeparam>
    public class RoomShapesHandler2<TNode, TConfiguration> : IRoomShapesHandler<ILayout<TNode, TConfiguration>, TNode, IntAlias<GridPolygon>>
        where TConfiguration : IConfiguration<IntAlias<GridPolygon>, IntVector2, TNode>, ISmartCloneable<TConfiguration>, new()
    {
        private readonly IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces;
        private readonly TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> intAliasMapping;
        private readonly IMapDescription<TNode> mapDescription;
        private readonly IGraph<TNode> stageOneGraph;
        private readonly RepeatMode? repeatModeOverride;
        private RoomTemplateInfo[] roomTemplateInstanceInfo;

        public RoomShapesHandler2(
            IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces,
            TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> intAliasMapping,
            IMapDescription<TNode> mapDescription,
            RepeatMode? repeatModeOverride = null)
        {
            this.configurationSpaces = configurationSpaces;
            this.intAliasMapping = intAliasMapping;
            this.mapDescription = mapDescription;
            this.repeatModeOverride = repeatModeOverride;
            stageOneGraph = mapDescription.GetStageOneGraph();

            Initialize();
        }

        /// <summary>
        /// We want to make sure that if a room template is set to no repeat and has multiple transformations,
        /// then all the different transformations are discarded. To make it possible, for each room template instance,
        /// we store the corresponding room template together with all its transformations, so we are able to quickly
        /// get all the required information.
        /// </summary>
        private void Initialize()
        {
            roomTemplateInstanceInfo = new RoomTemplateInfo[intAliasMapping.Count];

            foreach (var roomTemplate in intAliasMapping.Keys.Select(x => x.RoomTemplate).Distinct())
            {
                var aliases = intAliasMapping
                    .Where(x => x.Key.RoomTemplate == roomTemplate)
                    .Select(x => x.Value)
                    .ToList();

                var roomTemplateInfo = new RoomTemplateInfo(aliases, roomTemplate);

                foreach (var intAlias in aliases)
                {
                    roomTemplateInstanceInfo[intAlias.Alias] = roomTemplateInfo;
                }
            }
        }

        /// <summary>
        /// Gets all the possible shapes based on repeat modes.
        /// If zero shapes are found and tryToFixEmpty is set to true, we try to lower the requirements and e.g. use
        /// only the NoImmediate mode instead of the NoRepeat mode.
        /// </summary>
        public List<IntAlias<GridPolygon>> GetPossibleShapesForNode(ILayout<TNode, TConfiguration> layout, TNode node, bool tryToFixEmpty = false)
        {
            if (mapDescription.GetRoomDescription(node) is CorridorRoomDescription)
            {
                return configurationSpaces.GetShapesForNode(node).ToList();
            }

            var shapes = GetPossibleShapesForNode(layout, node, repeatModeOverride);

            if (shapes.Count == 0 && tryToFixEmpty)
            {
                // Try to lower our requirements and use NoImmediate instead of NoRepeat rather than returning an empty list
                if (repeatModeOverride == null || repeatModeOverride == RepeatMode.NoRepeat)
                {
                    shapes = GetPossibleShapesForNode(layout, node, RepeatMode.NoImmediate);
                }

                // Try to lower our requirements and use AllowRepeat instead of returning an empty list
                if (shapes.Count == 0 && repeatModeOverride != RepeatMode.AllowRepeat)
                {
                    shapes = GetPossibleShapesForNode(layout, node, RepeatMode.AllowRepeat);
                }
            }

            return shapes;
        }

        public IntAlias<GridPolygon> GetRandomShapeWithoutConstraintsDoNotUse(TNode node)
        {
            return configurationSpaces.GetRandomShape(node);
        }

        public bool CanPerturbShapeDoNotUse(TNode node)
        {
            return configurationSpaces.CanPerturbShape(node);
        }

        private List<IntAlias<GridPolygon>> GetPossibleShapesForNode(ILayout<TNode, TConfiguration> layout, TNode node, RepeatMode? modeOverride)
        {
            var shapesForNode = new HashSet<IntAlias<GridPolygon>>(configurationSpaces.GetShapesForNode(node));

            foreach (var configuration in layout.GetAllConfigurations())
            {
                if (configuration.Room.Equals(node))
                {
                    continue;
                }

                var roomTemplateInfo = roomTemplateInstanceInfo[configuration.RoomShape.Alias];
                var repeatMode = modeOverride ?? roomTemplateInfo.RoomTemplate.RepeatMode;

                if (repeatMode == RepeatMode.NoRepeat || (repeatMode == RepeatMode.NoImmediate && stageOneGraph.HasEdge(node, configuration.Room)))
                {
                    foreach (var alias in roomTemplateInfo.Aliases)
                    {
                        shapesForNode.Remove(alias);
                    }
                }
            }

            return shapesForNode.ToList();
        }

        private class RoomTemplateInfo
        {
            public List<IntAlias<GridPolygon>> Aliases { get; }

            public RoomTemplate RoomTemplate { get; }

            public RoomTemplateInfo(List<IntAlias<GridPolygon>> aliases, RoomTemplate roomTemplate)
            {
                Aliases = aliases;
                RoomTemplate = roomTemplate;
            }
        }
    }
}