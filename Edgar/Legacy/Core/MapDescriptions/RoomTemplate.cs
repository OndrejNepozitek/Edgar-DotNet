using System.Collections.Generic;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Core.MapDescriptions
{
    /// <summary>
	/// Description of a room.
	/// </summary>
	public class RoomTemplate
	{
		public PolygonGrid2D Shape { get; }

		public IDoorMode DoorsMode { get; }

        public List<TransformationGrid2D> AllowedTransformations { get; }

		public RepeatMode RepeatMode { get; }

		public string Name { get; }

		public RoomTemplate(PolygonGrid2D shape, IDoorMode doorsMode, List<TransformationGrid2D> allowedTransformations = null, RepeatMode repeatMode = RepeatMode.AllowRepeat, string name = null)
		{
			Shape = shape;
			DoorsMode = doorsMode;
            AllowedTransformations = allowedTransformations ?? new List<TransformationGrid2D>() { TransformationGrid2D.Identity };
            RepeatMode = repeatMode;
            Name = name ?? "Room template";
        }
	}
}