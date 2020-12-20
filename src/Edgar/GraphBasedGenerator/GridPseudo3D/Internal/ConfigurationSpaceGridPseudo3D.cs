using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D.Internal;

namespace Edgar.GraphBasedGenerator.GridPseudo3D.Internal
{
    public class ConfigurationSpaceGridPseudo3D
    {
        public Dictionary<int, ConfigurationSpaceGrid2D> ConfigurationSpaces { get; }

        public ConfigurationSpaceGridPseudo3D(Dictionary<int, ConfigurationSpaceGrid2D> configurationSpaces)
        {
            ConfigurationSpaces = configurationSpaces;
        }
    }
}