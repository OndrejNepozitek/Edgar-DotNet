namespace MapGeneration.Core
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;
	public class RoomDescription : IRoomDescription
	{
		public GridPolygon Shape { get; }
		public DoorsMode DoorsMode { get; }
		public int MinimumOverlap { get; }

		public IReadOnlyCollection<IntVector2> GetDoorsPoints()
		{
			throw new System.NotImplementedException();
		}

		public IReadOnlyCollection<IntLine> GetDoorsLines()
		{
			throw new System.NotImplementedException();
		}
	}
}