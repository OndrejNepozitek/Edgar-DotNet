using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Core.MapDescriptions
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
        public PolygonGrid2D RoomShape { get; }

        /// <summary>
        /// Room shape after transformation.
        /// </summary>
        public IntAlias<PolygonGrid2D> RoomShapeAlias { get; set; }

        /// <summary>
        /// Door lines.
        /// </summary>
        public List<DoorLine> DoorLines { get; }

        /// <summary>
        /// All transformations that led to this room shape.
        /// </summary>
        public List<TransformationGrid2D> Transformations { get; }

        public RoomTemplateInstance(RoomTemplate roomTemplate, PolygonGrid2D roomShape,
            List<TransformationGrid2D> transformations, List<DoorLine> doorLines)
        {
            RoomTemplate = roomTemplate;
            RoomShape = roomShape;
            Transformations = transformations;
            DoorLines = doorLines;
        }

        public RoomTemplateInstance(RoomTemplate roomTemplate, PolygonGrid2D roomShape,
            TransformationGrid2D transformation, List<DoorLine> doorLines)
            : this(roomTemplate, roomShape, new List<TransformationGrid2D>() {transformation}, doorLines)
        {
        }

        #region Equals

        protected bool Equals(RoomTemplateInstance other)
        {
            return Equals(RoomTemplate, other.RoomTemplate) && Equals(RoomShape, other.RoomShape) &&
                   DoorLines.SequenceEqual(other.DoorLines) && Transformations.SequenceEqual(other.Transformations);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RoomTemplateInstance) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (RoomTemplate != null ? RoomTemplate.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (RoomShape != null ? RoomShape.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(RoomTemplateInstance left, RoomTemplateInstance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoomTemplateInstance left, RoomTemplateInstance right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}