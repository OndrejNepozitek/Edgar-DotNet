namespace MapGeneration.Interfaces
{
	using System.Collections.Generic;

	public interface ILayout<TNode, TPolygon, TPosition>
	{
		IConfiguration<TPolygon, TPosition> GetConfiguration(TNode node);

		IEnumerable<IConfiguration<TPolygon, TPosition>> GetConfigurations();

		IEnumerable<IRoom<TNode, TPolygon, TPosition>> GetRooms();
	}
}
