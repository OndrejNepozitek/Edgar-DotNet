namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.ObjectModel;

	public interface IPolygon<TPoint>
	{
		void AddPoint(TPoint point);

		ReadOnlyCollection<TPoint> GetPoints();
	}
}
