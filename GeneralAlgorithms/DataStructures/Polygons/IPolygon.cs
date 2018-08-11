namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.ObjectModel;

	/// <summary>
	/// Represents a polygon.
	/// </summary>
	/// <typeparam name="TPoint"></typeparam>
	public interface IPolygon<TPoint>
	{
		ReadOnlyCollection<TPoint> GetPoints();
	}
}
