using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;

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

        public RoomTemplate(GridPolygon shape, IDoorMode doorsMode)
        {
            Shape = new GridPolygon(shape.GetPoints());
            DoorsMode = doorsMode;
        }

		public RoomTemplate(GridPolygon shape, IDoorMode doorsMode, List<Transformation> allowedTransformations)
		{
			Shape = shape; // TODO: should I create a copy?
			DoorsMode = doorsMode;
            AllowedTransformations = allowedTransformations;
        }
	}
}