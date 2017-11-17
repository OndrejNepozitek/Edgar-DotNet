namespace MapGeneration.Layouts
{
	public interface ILayout<TPolygon>
	{
		bool IsValid();

		float GetEnergy();

		float GetDifference(ILayout<TPolygon> other);
	}
}
