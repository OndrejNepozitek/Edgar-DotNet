namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;

	public interface IPolygonUtils<T>
	{
		bool CheckIntegrity(T polygon);

		T Rotate(T polygon, int degrees);

		IEnumerable<T> GetAllRotations(T polygon);
	}
}
