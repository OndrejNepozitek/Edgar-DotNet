namespace MapGeneration.Core.MapLayouts
{
    using MapGeneration.Core.MapLayouts.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a complete layout of a map. 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class MapLayout<TNode> : IMapLayout<TNode>
	{
		public List<IRoom<TNode>> Rooms { get; }

		public MapLayout(List<IRoom<TNode>> rooms)
		{
			Rooms = rooms;
		}
	}
}