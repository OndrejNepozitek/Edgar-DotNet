namespace MapGeneration.Interfaces
{
	using System.Collections.Generic;

	public interface IMapLayout<TNode, TPolygon, TPosition>
	{
		IRoom<TNode, TPolygon, TPosition> GetRoom(TNode node);

		IEnumerable<IRoom<TNode, TPolygon, TPosition>> GetRooms();
	}
}