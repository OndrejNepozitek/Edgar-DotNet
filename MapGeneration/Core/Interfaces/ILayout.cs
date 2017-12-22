namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;

	public interface ILayout<TNode>
	{
		bool GetConfiguration(TNode node, out Configuration configuration);

		void SetConfiguration(TNode node, Configuration configuration);

		IEnumerable<Configuration> GetConfigurations();

		IEnumerable<IRoom<TNode>> GetRooms();

		// IEnumerable<TDoor> GetDoors();

		ILayout<TNode> Clone();
	}
}