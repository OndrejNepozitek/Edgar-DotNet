namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons
{
	/// <summary>
	/// Represents types that can normalize given polygons.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IPolygonUtils<T>
	{ 
		/// <summary>
		/// Normalizes a given polygon.
		/// </summary>
		/// <param name="polygon"></param>
		/// <returns></returns>
		T NormalizePolygon(T polygon);
	}
}
