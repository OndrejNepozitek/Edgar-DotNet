namespace MapGeneration.Core
{
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class RoomDescription : IRoomDescription
	{
		public GridPolygon Shape { get; }

		public DoorsMode DoorsMode { get; }
	}
}