namespace MapGeneration.Interfaces.Core.MapDescriptions
{
	using Doors;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoomDescription
	{
		GridPolygon Shape { get; }

		IDoorMode DoorsMode { get; }
	}
}