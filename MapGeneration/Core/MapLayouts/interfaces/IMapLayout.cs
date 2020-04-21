namespace MapGeneration.Core.MapLayouts.Interfaces
{
	using System.Collections.Generic;

    /// <summary>
    /// Represents a complete layout of a map. 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public interface IMapLayout<TNode>
	{
		List<IRoom<TNode>> Rooms { get; }
	}
}