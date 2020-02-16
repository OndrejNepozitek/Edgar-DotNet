using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.LayoutOperations;

namespace MapGeneration.Core.MapDescriptions
{
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Doors;
	using Interfaces.Core.MapDescriptions;

	/// <summary>
	/// Description of a room.
	/// </summary>
	public class RoomTemplate : IRoomTemplate
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