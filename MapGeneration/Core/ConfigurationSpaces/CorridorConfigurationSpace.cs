using System;
using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Core.ConfigurationSpaces
{
    public class CorridorConfigurationSpace : ConfigurationSpace
    {
        public Dictionary<IntVector2, List<IntVector2>> PositionToCorridorPositionsMapping { get; set; }
    }
}