namespace MapGeneration.Core
{
	using System.Collections.Generic;
	using Interfaces;

	public class MapLayout<TNode> : IMapLayout<TNode>
	{
		private readonly IEnumerable<IRoom<TNode>> rooms;

		public MapLayout(IEnumerable<IRoom<TNode>> rooms)
		{
			this.rooms = rooms;
		}

		public IEnumerable<IRoom<TNode>> GetRooms()
		{
			return rooms;
		}
	}
}