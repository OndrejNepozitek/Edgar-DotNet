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
		public List<Room<TNode>> Rooms { get; }

		public LevelGrid2D(List<Room<TNode>> rooms)
		{
			Rooms = rooms;
		}
	}
}