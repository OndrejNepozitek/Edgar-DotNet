namespace MapGeneration.Interfaces.Core.MapLayouts
{
	using System.Collections.Generic;

	/// <summary>
	/// Represent a complete layout of a map. 
	/// </summary>
	/// <remarks>
	/// See <see cref="ILayout{TNode}"/> for a representation that is used
	/// when evolving layouts.
	/// TODO: is there a better way to distinguish these representations?
	/// </remarks>
	/// <typeparam name="TNode"></typeparam>
	public interface IMapLayout<TNode>
	{
		IEnumerable<IRoom<TNode>> Rooms { get; }
	}
}