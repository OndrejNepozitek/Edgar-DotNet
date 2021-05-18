using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public class LevelGeometryData<TRoom>
    {
        public Dictionary<TRoom, RoomDescriptionGrid2D> RoomDescriptions { get; set; }

        public List<RoomTemplateGrid2D> RoomTemplates { get; set; }

        public Dictionary<RoomTemplateGrid2D, List<RoomTemplateInstanceGrid2D>> RoomTemplateInstances { get; set; }

        public TwoWayDictionary<RoomTemplateInstanceGrid2D, IntAlias<PolygonGrid2D>> RoomTemplateInstanceToPolygonMapping { get; set; }

        public static LevelGeometryData<TRoom> CreateBackwardsCompatible(
            ILevelDescription<TRoom> levelDescription,
            Func<RoomTemplateGrid2D, List<RoomTemplateInstanceGrid2D>> roomTemplateInstanceFactory)
        {
            var roomDescriptions = levelDescription
                .GetGraph()
                .Vertices
                .ToDictionary(x => x, x => (RoomDescriptionGrid2D)levelDescription.GetRoomDescription(x));
            var roomTemplates = roomDescriptions
                .Values
                .SelectMany(x => x.RoomTemplates)
                .Distinct()
                .ToList();
            var roomTemplateInstances = roomTemplates.ToDictionary(x => x, roomTemplateInstanceFactory);
            var roomTemplateInstancesMapping = roomTemplateInstances
                .SelectMany(x => x.Value)
                .CreateIntMapping();
            var intAliasMapping = new TwoWayDictionary<RoomTemplateInstanceGrid2D, IntAlias<PolygonGrid2D>>();

            foreach (var shape1 in roomTemplateInstancesMapping.Keys)
            {
                foreach (var shape2 in roomTemplateInstancesMapping.Keys)
                {
                    if (!intAliasMapping.ContainsKey(shape1))
                    {
                        var newAlias = new IntAlias<PolygonGrid2D>(intAliasMapping.Count, shape1.RoomShape);
                        intAliasMapping.Add(shape1, newAlias);
                        shape1.RoomShapeAlias = newAlias;
                    }
                    if (!intAliasMapping.ContainsKey(shape2))
                    {
                        var newAlias = new IntAlias<PolygonGrid2D>(intAliasMapping.Count, shape2.RoomShape);
                        intAliasMapping.Add(shape2, newAlias);
                        shape2.RoomShapeAlias = newAlias;
                    }
                }
            }

            // TODO: remove when possible
            foreach (var pair in intAliasMapping)
            {
                pair.Key.RoomShapeAlias = pair.Value;
            }

            var data = new LevelGeometryData<TRoom>()
            {
                RoomDescriptions = roomDescriptions,
                RoomTemplates = roomTemplates,
                RoomTemplateInstances = roomTemplateInstances,
                RoomTemplateInstanceToPolygonMapping = intAliasMapping,
            };

            return data;
        }
    }
}