namespace GeneralAlgorithms.Algorithms.Polygons
{
	using DataStructures.Common;
	using DataStructures.Polygons;

	public interface IPolygonOverlap
	{
		bool DoOverlap(GridPolygon polygon1, IntVector2 position1, GridPolygon polygon2, IntVector2 position2);

		bool DoOverlap(GridRectangle rectangle1, GridRectangle rectangle2);

		int OverlapArea(GridPolygon polygon1, IntVector2 position1, GridPolygon polygon2, IntVector2 position2);

		bool DoTouch(GridPolygon polygon1, IntVector2 position1, GridPolygon polygon2, IntVector2 position2, int minimumLength = 1);
	}
}