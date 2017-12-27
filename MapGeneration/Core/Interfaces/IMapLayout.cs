namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;

	public interface IMapLayout<out TNode>
	{
		IEnumerable<IRoom<TNode>> GetRooms();
	}
}