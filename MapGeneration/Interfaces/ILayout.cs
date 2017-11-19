namespace MapGeneration.Interfaces
{
	public interface ILayout<TPolygon>
	{
		// TODO: should not be here as it depends on the current task
		float GetEnergy();

		// TODO: possibly the same problem as above
		float GetDifference(ILayout<TPolygon> other);
	}
}
