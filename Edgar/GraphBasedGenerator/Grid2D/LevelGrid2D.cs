using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a complete layout of a map. 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class LevelGrid2D<TNode>
	{
		public List<RoomGrid2D<TNode>> Rooms { get; }

		public LevelGrid2D(List<RoomGrid2D<TNode>> rooms)
		{
			Rooms = rooms;
		}
	}
}