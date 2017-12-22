namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoomDescription
	{
		GridPolygon Shape { get; }

		DoorsMode DoorsMode { get; }

		int MinimumOverlap { get; }

		IReadOnlyCollection<IntVector2> GetDoorsPoints();

		IReadOnlyCollection<IntLine> GetDoorsLines();
	}
}