namespace MapGeneration.Interfaces
{
	public interface IRoom<TNode, TPolygon, TPosition>
	{
		TNode Node { get; }

		IConfiguration<TPolygon, TPosition> Configuration { get; }
	}
}