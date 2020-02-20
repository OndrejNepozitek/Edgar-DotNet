using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Doors.Interfaces;
using MapGeneration.Core.LayoutOperations;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Core.MapDescriptions
{
	using GeneralAlgorithms.DataStructures.Polygons;

    /// <summary>
	/// Description of a room.
	/// </summary>
	public class RoomTemplate
	{
		public GridPolygon Shape { get; }

		public IDoorMode DoorsMode { get; }

        public List<Transformation> AllowedTransformations { get; }

		public RepeatMode RepeatMode { get; }



		public RoomTemplate(GridPolygon shape, IDoorMode doorsMode, List<Transformation> allowedTransformations = null, RepeatMode repeatMode = RepeatMode.AllowRepeat)
		{
			Shape = shape;
			DoorsMode = doorsMode;
            AllowedTransformations = allowedTransformations ?? new List<Transformation>() { Transformation.Identity };
            RepeatMode = repeatMode;
        }
	}
}