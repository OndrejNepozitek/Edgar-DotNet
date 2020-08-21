using System.Collections.Generic;
using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Holds information about a single room in the final layout.
    /// </summary>
    public class LayoutRoomGrid2D<TRoom>
	{
        /// <summary>
        /// Corresponding room from the level description.
        /// </summary>
        public TRoom Room { get; }

        /// <summary>
        /// Description of the corresponding room.
        /// </summary>
        public RoomDescriptionGrid2D RoomDescription { get; }

        /// <summary>
        /// Outline of the room.
        /// </summary>
        /// <remarks>
        /// The outline does not reflect the position of the room.
        /// To get a correctly positioned outline, the Position vector must be added to the Outline polygon.
        ///
        /// If there was no transformation applied to the room template (i.e. the Transformation property is equal to Identity),
        /// the outline polygon is equal to the RoomTemplate.Outline polygon. If a transformation was applied (i.e. Rotate 90),
        /// the outline polygon is equal to RoomTemplate.Outline.Transform(transformation).
        /// </remarks>
        public PolygonGrid2D Outline { get; }

        /// <summary>
        /// Position of the room.
        /// </summary>
        public Vector2Int Position { get; }
        
        /// <summary>
        /// Room template used for this room.
        /// </summary>
        public RoomTemplateGrid2D RoomTemplate { get; }
        
        /// <summary>
        /// Whether it is a corridor room or not.
        /// </summary>
        public bool IsCorridor { get; }

        /// <summary>
        /// Doors leading to neighboring rooms.
        /// </summary>
        public List<LayoutDoorGrid2D<TRoom>> Doors { get; set; }

        /// <summary>
        /// Transformation of the room template.
        /// </summary>
        /// <remarks>
        /// If there were multiple transformations allowed for the room template,
        /// this property contains the transformation that was used for this instance
        /// of the room template.
        /// </remarks>
        public TransformationGrid2D Transformation { get; }

        public LayoutRoomGrid2D(TRoom room, PolygonGrid2D outline, Vector2Int position, bool isCorridor, RoomTemplateGrid2D roomTemplate, RoomDescriptionGrid2D roomDescription, TransformationGrid2D transformation)
		{
			Room = room;
			Outline = outline;
			Position = position;
			IsCorridor = isCorridor;
			RoomTemplate = roomTemplate;
			Transformation = transformation;
            RoomDescription = roomDescription;
        }
	}
}