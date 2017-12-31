namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoomDescription
	{
		GridPolygon Shape { get; }

		DoorsMode DoorsMode { get; }
	}
}