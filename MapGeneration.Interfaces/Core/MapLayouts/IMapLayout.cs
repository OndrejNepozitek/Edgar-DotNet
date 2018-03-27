namespace MapGeneration.Interfaces.Core.MapLayouts
{
	using System.Collections.Generic;
	using Layouts;

	/// <summary>
	/// Represents a complete layout of a map. 
	/// </summary>
	/// <remarks>
	/// See <see cref="ILayout{TNode,TConfiguration}"/> for a representation that is used
	/// when evolving layouts.
	/// </remarks>
	/// <typeparam name="TNode"></typeparam>
	public interface IMapLayout<TNode>
	{
		IEnumerable<IRoom<TNode>> Rooms { get; }
	}
}