using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.ConfigurationSpaces
{
    /// <summary>
    /// Configuration space of a pair of polygons.
    /// </summary>
    public class ConfigurationSpace
    {
        public List<OrthogonalLineGrid2D> Lines;

        public List<Tuple<OrthogonalLineGrid2D, DoorLine>> ReverseDoors;
    }
}