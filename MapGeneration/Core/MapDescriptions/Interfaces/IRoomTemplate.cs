using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Doors.Interfaces;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Core.MapDescriptions.Interfaces
{
	using GeneralAlgorithms.DataStructures.Polygons;

    /// <summary>
	/// Description of a room.
	/// </summary>
	public interface IRoomTemplate
	{
		GridPolygon Shape { get; }

		IDoorMode DoorsMode { get; }

        List<Transformation> AllowedTransformations { get; }

		RepeatMode RepeatMode { get; }

		string Name { get; }
	}
}