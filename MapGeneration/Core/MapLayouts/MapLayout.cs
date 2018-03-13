namespace MapGeneration.Core.MapLayouts
{
	using System.Collections.Generic;
	using Interfaces.Core.MapLayouts;

	/// <inheritdoc />
	public class MapLayout<TNode> : IMapLayout<TNode>
	{
		public IEnumerable<IRoom<TNode>> Rooms { get; }

		public MapLayout(IEnumerable<IRoom<TNode>> rooms)
		{
			Rooms = rooms;
		}
	}
}