namespace MapGeneration.Grid.Descriptions
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class RoomDescription
	{
		private GridPolygon polygon;
		
		private List<IntVector2> doorPositions;

		public DoorsModeE DoorsMode { get; private set; }

		public int MinimumOverlap { get; private set; }

		public RoomDescription(GridPolygon polygon)
		{
			this.polygon = new GridPolygon(polygon.GetPoints());
		}

		public RoomDescription(IEnumerable<IntVector2> points)
		{
			polygon = new GridPolygon(points);
		}

		public void SetRooms(int length, int minimumOverlap)
		{
			DoorsMode = DoorsModeE.OverlapBased;
			MinimumOverlap = minimumOverlap;
		}

		public void SetRooms(int length, List<IntVector2> points)
		{
			DoorsMode = DoorsModeE.SinglePoints;
			doorPositions = points;
		}

		public IReadOnlyCollection<IntVector2> GetDoorPoints()
		{
			return doorPositions.AsReadOnly();
		}

		public enum DoorsModeE
		{
			NotSpecified,
			SinglePoints,
			OverlapBased
		}
	}
}