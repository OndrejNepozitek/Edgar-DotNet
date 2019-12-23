using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Interfaces.Core.Doors;
using MapGeneration.Interfaces.Core.MapDescriptions;

namespace MapGeneration.Core.MapDescriptions
{
	public class RoomTemplateInstance
    {
        /// <summary>
        /// Room description.
        /// </summary>
        public IRoomTemplate RoomTemplate { get; }

        /// <summary>
        /// Room shape after transformation.
        /// </summary>
        public GridPolygon RoomShape { get; }

        /// <summary>
        /// Door lines.
        /// </summary>
        public List<IDoorLine> DoorLines { get; }

        /// <summary>
        /// All transformations that led to this room shape.
        /// </summary>
        public List<Transformation> Transformations { get; }

        public RoomTemplateInstance(IRoomTemplate roomTemplate, GridPolygon roomShape, List<Transformation> transformations, List<IDoorLine> doorLines)
        {
            RoomTemplate = roomTemplate;
            RoomShape = roomShape;
            Transformations = transformations;
            DoorLines = doorLines;
        }

        public RoomTemplateInstance(IRoomTemplate roomTemplate, GridPolygon roomShape, Transformation transformation, List<IDoorLine> doorLines)
            : this(roomTemplate, roomShape, new List<Transformation>() { transformation }, doorLines)
        {

        }
    }
}