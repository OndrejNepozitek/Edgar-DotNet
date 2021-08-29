using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    /// <summary>
    /// This class is used to compute which doors correspond to a given relative position of two neighboring rooms.
    /// </summary>
    public class ConfigurationSpaceSourceGrid2D
    {
        /// <summary>
        /// Line from the configuration space.
        /// </summary>
        public OrthogonalLineGrid2D ConfigurationSpaceLine { get; }

        /// <summary>
        /// Door line from the fixed room template.
        /// </summary>
        public DoorLineGrid2D DoorLine { get; }

        /// <summary>
        /// Door line from the moving room template.
        /// </summary>
        public DoorLineGrid2D OtherDoorLine { get; }

        public ConfigurationSpaceSourceGrid2D(OrthogonalLineGrid2D configurationSpaceLine, DoorLineGrid2D doorLine, DoorLineGrid2D otherDoorLine)
        {
            ConfigurationSpaceLine = configurationSpaceLine;
            DoorLine = doorLine;
            OtherDoorLine = otherDoorLine;
        }
    }
}