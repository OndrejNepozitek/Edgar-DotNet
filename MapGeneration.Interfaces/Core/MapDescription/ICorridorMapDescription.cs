namespace MapGeneration.Interfaces.Core.MapDescription
{
	public interface ICorridorMapDescription<TNode> : IMapDescription<TNode>
	{
		bool IsCorridorRoom(TNode room);
	}
}