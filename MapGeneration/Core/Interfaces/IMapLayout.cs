namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;

	public interface IMapLayout<TNode>
	{
		IEnumerable<IRoom<TNode>> GetRooms();
	}
}