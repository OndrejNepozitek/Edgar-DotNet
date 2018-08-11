namespace MapGeneration.Interfaces.Core.LayoutOperations
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents layout operations that are meant to work with corridors.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	public interface ILayoutOperationsWithCorridors<in TLayout, TNode> : IChainBasedLayoutOperations<TLayout, TNode>
	{
		/// <summary>
		/// Add corridors from a given chain to a given layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		bool AddCorridors(TLayout layout, IList<TNode> chain);
	}
}