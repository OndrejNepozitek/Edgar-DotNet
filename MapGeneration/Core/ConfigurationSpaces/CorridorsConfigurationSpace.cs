using System;
using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Core.ConfigurationSpaces
{
    public class CorridorsConfigurationSpace<TShape> : ConfigurationSpace
    {
        public Dictionary<IntVector2, List<Tuple<IntVector2, TShape>>> PositionToCorridorMapping { get; set; }
    }
}