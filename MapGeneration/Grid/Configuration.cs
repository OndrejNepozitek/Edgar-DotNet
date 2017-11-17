namespace MapGeneration.ConfigurationSpaces
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public struct GridConfiguration
	{
		public readonly GridPolygon Polygon;
		public readonly IntVector2 Position;

		public GridConfiguration(GridPolygon polygon, IntVector2 position)
		{
			Polygon = polygon;
			Position = position;
		}
	}
}