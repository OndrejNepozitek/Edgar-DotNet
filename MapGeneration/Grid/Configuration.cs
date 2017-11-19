namespace MapGeneration.Grid
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public struct Configuration
	{
		public readonly GridPolygon Polygon;
		public readonly IntVector2 Position;

		public Configuration(GridPolygon polygon, IntVector2 position)
		{
			Polygon = polygon;
			Position = position;
		}
	}
}