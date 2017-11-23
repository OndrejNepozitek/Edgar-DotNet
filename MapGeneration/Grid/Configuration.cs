namespace MapGeneration.Grid
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public struct Configuration
	{
		public readonly GridPolygon Polygon;
		public readonly IntVector2 Position;
		public readonly int WrongNeighbours;

		public Configuration(GridPolygon polygon, IntVector2 position, int wrongNeighbours = 0)
		{
			Polygon = polygon;
			Position = position;
			WrongNeighbours = wrongNeighbours;
		}
	}
}