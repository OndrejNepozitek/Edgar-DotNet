using System.Collections.ObjectModel;

namespace Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons
{
    /// <summary>
	/// Represents a polygon.
	/// </summary>
	/// <typeparam name="TPoint"></typeparam>
	public interface IPolygon<TPoint>
	{
		ReadOnlyCollection<TPoint> GetPoints();
	}
}
