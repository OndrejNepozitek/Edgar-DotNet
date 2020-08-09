using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator
{
    /// <summary>
    /// Represents a complete layout of a map. 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class LevelGrid2D<TNode>
	{
		public List<Room<TNode>> Rooms { get; }

		public LevelGrid2D(List<Room<TNode>> rooms)
		{
			Rooms = rooms;
		}
	}
}