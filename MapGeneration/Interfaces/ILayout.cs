namespace MapGeneration.Interfaces
{
	using System.Collections.Generic;

	public interface ILayout<TNode, TPolygon, TPosition, TDoor>
	{
		IConfiguration<TPolygon, TPosition> GetConfiguration(TNode node);

		IEnumerable<IConfiguration<TPolygon, TPosition>> GetConfigurations();

		IEnumerable<IRoom<TNode, TPolygon, TPosition>> GetRooms();

		IEnumerable<TDoor> GetDoors();
	}
}
