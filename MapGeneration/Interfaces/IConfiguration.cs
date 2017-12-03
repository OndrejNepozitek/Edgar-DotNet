namespace MapGeneration.Interfaces
{
	public interface IConfiguration<TPolygon, TPosition>
	{
		TPolygon Shape { get; }

		TPosition Position { get; }
	}
}