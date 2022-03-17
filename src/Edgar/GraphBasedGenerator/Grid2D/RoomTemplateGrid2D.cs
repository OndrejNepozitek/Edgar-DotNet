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
        /// Name of the room template. 
        /// </summary>
        /// <remarks>
        /// Used mainly for debugging purposes.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Outline of the room template.
        /// </summary>
        public PolygonGrid2D Outline { get; }

        /// <summary>
        /// Available door positions of the room template.
        /// </summary>
        /// <remarks>
        /// See <see cref="SimpleDoorModeGrid2D"/> or <see cref="ManualDoorModeGrid2D"/> for available door modes.
        /// </remarks>
        public IDoorModeGrid2D Doors { get; }

        /// <summary>
        /// Settings regarding if the room template can be used multiple times in a level.
        /// </summary>
        /// <remarks>
        /// See <see cref="RoomTemplateRepeatMode"/>.
        /// </remarks>
        public RoomTemplateRepeatMode? RepeatMode { get; }

        /// <summary>
        /// How can the room template be transformed. 
        /// </summary>
        /// <remarks>
        /// By default only the Identity transformation is enabled which means that the room template stays as is.
        /// If e.g. the Rotate90 transformation is allowed, the generator may decide to rotate the room when it
        /// tries to find a valid layout.
        ///
        /// See <see cref="TransformationGrid2D"/>.
        /// </remarks>
        public List<TransformationGrid2D> AllowedTransformations { get; }

        /// <param name="outline">See the <see cref="Outline"/> property.</param>
        /// <param name="doors">See the <see cref="Doors"/> property.</param>
        /// <param name="name">See the <see cref="Name"/> property.</param>
        /// <param name="repeatMode">See the <see cref="RepeatMode"/> property.</param>
        /// <param name="allowedTransformations">See the <see cref="AllowedTransformations"/> property.</param>
        public RoomTemplateGrid2D(PolygonGrid2D outline, IDoorModeGrid2D doors, string name = null,
            RoomTemplateRepeatMode? repeatMode = null, List<TransformationGrid2D> allowedTransformations = null)
        {
            Outline = outline;
            Doors = doors;
            Name = name ?? "Room template";
            RepeatMode = repeatMode;
            AllowedTransformations = allowedTransformations ?? new List<TransformationGrid2D>()
                {TransformationGrid2D.Identity};
            ;
        }
    }
}