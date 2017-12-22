namespace MapGeneration.Interfaces
{
	public interface IRoom<out TNode, out TPolygon, out TPosition>
	{
		TNode Node { get; }

		TPolygon Shape { get; }

		TPosition Position { get; }
	}
}