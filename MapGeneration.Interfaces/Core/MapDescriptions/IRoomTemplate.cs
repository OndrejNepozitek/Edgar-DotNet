using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;

namespace MapGeneration.Interfaces.Core.MapDescriptions
{
	using Doors;
	using GeneralAlgorithms.DataStructures.Polygons;

	// TODO: why do we need to keep the interface?
	public interface IRoomTemplate
	{
		GridPolygon Shape { get; }

		IDoorMode DoorsMode { get; }

        List<Transformation> AllowedTransformations { get; }

        RepeatMode RepeatMode { get; }
	}
}