using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.Graphs;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.LayoutOperations.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    /// <summary>
    /// Class responsible for returning available shapes for a node based on used repeat mode.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TConfiguration"></typeparam>
    public class RoomShapesHandlerGrid2D<TNode, TConfiguration> : IRoomShapesHandler<ILayout<TNode, TConfiguration>, TNode, RoomTemplateInstanceGrid2D>, IRandomInjectable
        where TConfiguration : IRoomConfiguration<TNode>, IShapeConfiguration<RoomTemplateInstanceGrid2D>, ISmartCloneable<TConfiguration>, new()
    {
        private readonly TwoWayDictionary<RoomTemplateInstanceGrid2D, IntAlias<PolygonGrid2D>> intAliasMapping;
        private readonly ILevelDescription<TNode> mapDescription;
        private readonly IGraph<TNode> graphWithoutCorridors;
        private readonly RoomTemplateRepeatMode? repeatModeOverride;
        private readonly RoomTemplateRepeatMode? repeatModeDefault;
        private RoomTemplateInfo[] roomTemplateInstanceInfo;
        private readonly Dictionary<TNode, List<WeightedShape>> shapesForNodes;
        private Random random;

        public RoomShapesHandlerGrid2D(
            TwoWayDictionary<RoomTemplateInstanceGrid2D, IntAlias<PolygonGrid2D>> intAliasMapping,
            ILevelDescription<TNode> mapDescription, Dictionary<TNode, List<WeightedShape>> shapesForNodes, RoomTemplateRepeatMode? repeatModeOverride = null, RoomTemplateRepeatMode? repeatModeDefault = null)
        {
            this.intAliasMapping = intAliasMapping;
            this.mapDescription = mapDescription;
            this.shapesForNodes = shapesForNodes;
            this.repeatModeDefault = repeatModeDefault;
            this.repeatModeOverride = repeatModeOverride;
            graphWithoutCorridors = mapDescription.GetGraphWithoutCorridors();

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
        public List<RoomTemplateInstanceGrid2D> GetPossibleShapesForNode(ILayout<TNode, TConfiguration> layout, TNode node, bool tryToFixEmpty = false)
        {
            if (mapDescription.GetRoomDescription(node).IsCorridor)
            {
                return shapesForNodes[node].Select(x => intAliasMapping.GetByValue(x.Shape)).ToList();
                // return configurationSpaces.GetShapesForNode(node).Select(x => intAliasMapping.GetByValue(x)).ToList();
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

        public RoomTemplateInstanceGrid2D GetRandomShapeWithoutConstraintsDoNotUse(TNode node)
        {
            // TODO: slow
            return intAliasMapping.GetByValue(shapesForNodes[node].GetWeightedRandom(x => x.Weight, random).Shape);
        }

        public bool CanPerturbShapeDoNotUse(TNode node)
        {
            return shapesForNodes[node].Count > 2;
            // return configurationSpaces.CanPerturbShape(node);
        }

        private List<RoomTemplateInstanceGrid2D> GetPossibleShapesForNode(ILayout<TNode, TConfiguration> layout, TNode node, RoomTemplateRepeatMode? modeOverride)
        {
            var shapesForNode = new HashSet<IntAlias<PolygonGrid2D>>(shapesForNodes[node].Select(x => x.Shape));

            foreach (var configuration in layout.GetAllConfigurations())
            {
                // TODO: make better
                if (configuration.Room.Equals(node) || mapDescription.GetRoomDescription(configuration.Room).IsCorridor)
                {
                    continue;
                }

                var polygon = intAliasMapping[configuration.RoomShape];
                var roomTemplateInfo = roomTemplateInstanceInfo[polygon.Alias];
                var repeatMode = modeOverride ?? roomTemplateInfo.RoomTemplate.RepeatMode ?? repeatModeDefault;

                if (repeatMode == RoomTemplateRepeatMode.NoRepeat || (repeatMode == RoomTemplateRepeatMode.NoImmediate && graphWithoutCorridors.HasEdge(node, configuration.Room)))
                {
                    foreach (var alias in roomTemplateInfo.Aliases)
                    {
                        shapesForNode.Remove(alias);
                    }
                }
            }

            return shapesForNode.Select(x => intAliasMapping.GetByValue(x)).ToList();
        }

        private class RoomTemplateInfo
        {
            public List<IntAlias<PolygonGrid2D>> Aliases { get; }

            public RoomTemplateGrid2D RoomTemplate { get; }

            public RoomTemplateInfo(List<IntAlias<PolygonGrid2D>> aliases, RoomTemplateGrid2D roomTemplate)
            {
                Aliases = aliases;
                RoomTemplate = roomTemplate;
            }
        }

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }
    }
}