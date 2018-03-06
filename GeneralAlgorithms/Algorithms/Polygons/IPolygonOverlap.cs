namespace GeneralAlgorithms.Algorithms.Polygons
{
	using DataStructures.Common;

	public interface IPolygonOverlap<in TShape>
	{
		bool DoOverlap(TShape polygon1, IntVector2 position1, TShape polygon2, IntVector2 position2);

		int OverlapArea(TShape polygon1, IntVector2 position1, TShape polygon2, IntVector2 position2);

		bool DoTouch(TShape polygon1, IntVector2 position1, TShape polygon2, IntVector2 position2, int minimumLength = 1);
	}
}