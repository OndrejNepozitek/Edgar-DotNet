namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.ObjectModel;

	public interface IPolygon<T>
	{
		void AddPoint(T point);

		ReadOnlyCollection<T> GetPoints();
	}
}
