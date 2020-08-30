using System.Collections.Generic;

namespace Edgar.Legacy.Core.MapLayouts
{
    /// <summary>
    /// Represents a complete layout of a map. 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class MapLayout<TNode>
	{
		public List<Room<TNode>> Rooms { get; }

		public MapLayout(List<Room<TNode>> rooms)
		{
			Rooms = rooms;
		}
	}
}