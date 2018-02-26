namespace MapGeneration.Interfaces.Core
{
	using Doors;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoomDescription
	{
		GridPolygon Shape { get; }

		IDoorMode DoorsMode { get; }
	}
}