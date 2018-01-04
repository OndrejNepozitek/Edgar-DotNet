namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoomDescription
	{
		GridPolygon Shape { get; }

		IDoorMode DoorsMode { get; }
	}
}