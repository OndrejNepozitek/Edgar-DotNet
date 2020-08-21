using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	public class RoomTemplateInstanceGrid2D
    {
        /// <summary>
        /// Room description.
        /// </summary>
        public RoomTemplateGrid2D RoomTemplate { get; }

        /// <summary>
        /// Room shape after transformation.
        /// </summary>
        public PolygonGrid2D RoomShape { get; }

        /// <summary>
        /// Room shape after transformation.
        /// </summary>
        public IntAlias<PolygonGrid2D> RoomShapeAlias { get; set; }

        /// <summary>
        /// Door lines.
        /// </summary>
        public List<DoorLineGrid2D> DoorLines { get; }

        /// <summary>
        /// All transformations that led to this room shape.
        /// </summary>
        public List<TransformationGrid2D> Transformations { get; }

        public RoomTemplateInstanceGrid2D(RoomTemplateGrid2D roomTemplate, PolygonGrid2D roomShape, List<DoorLineGrid2D> doorLines, List<TransformationGrid2D> transformations)
        {
            RoomTemplate = roomTemplate;
            RoomShape = roomShape;
            DoorLines = doorLines;
            Transformations = transformations;
        }
    }
}