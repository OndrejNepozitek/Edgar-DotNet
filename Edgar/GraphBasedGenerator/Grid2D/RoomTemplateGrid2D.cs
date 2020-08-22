using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a room template - an outline of a room together with all possible door positions.
    /// </summary>
    public class RoomTemplateGrid2D
    {
        /// <summary>
        /// Name of the room template. Used mainly for debugging purposes.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Outline of the room template.
        /// </summary>
        public PolygonGrid2D Outline { get; }

        /// <summary>
        /// Doors of the room template.
        /// </summary>
        public IDoorModeGrid2D Doors { get; }

        /// <summary>
        /// Settings regarding if the room template can be used multiple times in a level.
        /// </summary>
        public RoomTemplateRepeatMode? RepeatMode { get; }

        /// <summary>
        /// How can be the room template transformed.
        /// </summary>
        public List<TransformationGrid2D> AllowedTransformations { get; }

        public RoomTemplateGrid2D(PolygonGrid2D outline, IDoorModeGrid2D doors, string name = null, RoomTemplateRepeatMode? repeatMode = null, List<TransformationGrid2D> allowedTransformations = null)
        {
            Outline = outline;
            Doors = doors;
            Name = name ?? "Room template";
            RepeatMode = repeatMode;
            AllowedTransformations = allowedTransformations ?? new List<TransformationGrid2D>() { TransformationGrid2D.Identity };;
        }
    }
}