using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;

namespace Edgar.LevelGeometry.Grid2D
{
    public class ConfigurationSpaceGrid2D
    {
        public IReadOnlyList<OrthogonalLine> Lines { get; }

        public bool HasInverseMapping { get; }
    }
}