namespace GeneralAlgorithms.Algorithms.Polygons
{
	public interface IPolygonUtils<T>
	{
		bool CheckIntegrity(T polygon);

		T NormalizePolygon(T polygon);
	}
}
