namespace MapGeneration.Interfaces.Core
{
	using System.Collections.Generic;

	public interface IMapLayout<TNode>
	{
		IEnumerable<IRoom<TNode>> GetRooms();
	}
}