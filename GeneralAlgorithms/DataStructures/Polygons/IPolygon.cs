namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.ObjectModel;

	public interface IPolygon<TPoint>
	{
		ReadOnlyCollection<TPoint> GetPoints();
	}
}
