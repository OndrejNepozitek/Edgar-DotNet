using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors;
using MapGeneration.Core.Doors.Interfaces;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Core.MapDescriptions
{
	public class RoomTemplateInstance
    {
        /// <summary>
        /// Room description.
        /// </summary>
        public RoomTemplate RoomTemplate { get; }

        /// <summary>
        /// Room shape after transformation.
        /// </summary>
        public GridPolygon RoomShape { get; }

        /// <summary>
        /// Door lines.
        /// </summary>
        public List<DoorLine> DoorLines { get; }

        /// <summary>
        /// All transformations that led to this room shape.
        /// </summary>
        public List<Transformation> Transformations { get; }

        public RoomTemplateInstance(RoomTemplate roomTemplate, GridPolygon roomShape, List<Transformation> transformations, List<DoorLine> doorLines)
        {
            RoomTemplate = roomTemplate;
            RoomShape = roomShape;
            Transformations = transformations;
            DoorLines = doorLines;
        }

        public RoomTemplateInstance(RoomTemplate roomTemplate, GridPolygon roomShape, Transformation transformation, List<DoorLine> doorLines)
            : this(roomTemplate, roomShape, new List<Transformation>() { transformation }, doorLines)
        {

        }
    }
}