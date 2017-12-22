namespace MapGeneration.Interfaces
{
	using System.Collections.Generic;

	public interface ILayout<TNode, out TPolygon, out TPosition, out TDoor, TConfiguration>
		where TConfiguration : IConfiguration<TPolygon, TPosition>
	{
		bool GetConfiguration(TNode node, out TConfiguration configuration);

		void SetConfiguration(TNode node, TConfiguration configuration);

		IEnumerable<TConfiguration> GetConfigurations();

		IEnumerable<IRoom<TNode, TPolygon, TPosition>> GetRooms();

		IEnumerable<TDoor> GetDoors();
	}
}
