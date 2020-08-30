using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Graphs;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.ConfigurationSpaces.Interfaces;
using Edgar.Legacy.Core.LayoutOperations.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutOperations
{
    /// <summary>
    /// Class responsible for returning available shapes for a node based on used repeat mode.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TConfiguration"></typeparam>
    public class RoomShapesHandler<TNode, TConfiguration> : IRoomShapesHandler<ILayout<TNode, TConfiguration>, TNode, IntAlias<PolygonGrid2D>>
        where TConfiguration : IConfiguration<IntAlias<PolygonGrid2D>, TNode>, ISmartCloneable<TConfiguration>, new()
    {
        private readonly IConfigurationSpaces<TNode, IntAlias<PolygonGrid2D>, TConfiguration, ConfigurationSpace> configurationSpaces;
        private readonly TwoWayDictionary<RoomTemplateInstance, IntAlias<PolygonGrid2D>> intAliasMapping;
        private readonly IMapDescription<TNode> mapDescription;
        private readonly IGraph<TNode> stageOneGraph;
        private readonly RoomTemplateRepeatMode? repeatModeOverride;
        private RoomTemplateInfo[] roomTemplateInstanceInfo;

        public RoomShapesHandler(
            IConfigurationSpaces<TNode, IntAlias<PolygonGrid2D>, TConfiguration, ConfigurationSpace> configurationSpaces,
            TwoWayDictionary<RoomTemplateInstance, IntAlias<PolygonGrid2D>> intAliasMapping,
            IMapDescription<TNode> mapDescription,
            RoomTemplateRepeatMode? repeatModeOverride = null)
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
        public List<IntAlias<PolygonGrid2D>> GetPossibleShapesForNode(ILayout<TNode, TConfiguration> layout, TNode node, bool tryToFixEmpty = false)
        {
            if (mapDescription.GetRoomDescription(node) is CorridorRoomDescription)
            {
                return configurationSpaces.GetShapesForNode(node).ToList();
            }

            var shapes = GetPossibleShapesForNode(layout, node, repeatModeOverride);

            if (shapes.Count == 0 && tryToFixEmpty)
            {
                // Try to lower our requirements and use NoImmediate instead of NoRepeat rather than returning an empty list
                if (repeatModeOverride == null || repeatModeOverride == RoomTemplateRepeatMode.NoRepeat)
                {
                    shapes = GetPossibleShapesForNode(layout, node, RoomTemplateRepeatMode.NoImmediate);
                }

                // Try to lower our requirements and use AllowRepeat instead of returning an empty list
                if (shapes.Count == 0 && repeatModeOverride != RoomTemplateRepeatMode.AllowRepeat)
                {
                    shapes = GetPossibleShapesForNode(layout, node, RoomTemplateRepeatMode.AllowRepeat);
                }
            }

            return shapes;
        }

        public IntAlias<PolygonGrid2D> GetRandomShapeWithoutConstraintsDoNotUse(TNode node)
        {
            return configurationSpaces.GetRandomShape(node);
        }

        public bool CanPerturbShapeDoNotUse(TNode node)
        {
            return configurationSpaces.CanPerturbShape(node);
        }

        private List<IntAlias<PolygonGrid2D>> GetPossibleShapesForNode(ILayout<TNode, TConfiguration> layout, TNode node, RoomTemplateRepeatMode? modeOverride)
        {
            var shapesForNode = new HashSet<IntAlias<PolygonGrid2D>>(configurationSpaces.GetShapesForNode(node));

            foreach (var configuration in layout.GetAllConfigurations())
            {
                if (configuration.Node.Equals(node))
                {
                    continue;
                }

                var roomTemplateInfo = roomTemplateInstanceInfo[configuration.ShapeContainer.Alias];
                var repeatMode = modeOverride ?? roomTemplateInfo.RoomTemplate.RoomTemplateRepeatMode;

                if (repeatMode == RoomTemplateRepeatMode.NoRepeat || (repeatMode == RoomTemplateRepeatMode.NoImmediate && stageOneGraph.HasEdge(node, configuration.Node)))
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
            public List<IntAlias<PolygonGrid2D>> Aliases { get; }

            public RoomTemplate RoomTemplate { get; }

            public RoomTemplateInfo(List<IntAlias<PolygonGrid2D>> aliases, RoomTemplate roomTemplate)
            {
                Aliases = aliases;
                RoomTemplate = roomTemplate;
            }
        }
    }
}