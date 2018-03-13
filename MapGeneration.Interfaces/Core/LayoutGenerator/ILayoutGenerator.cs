namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System.Collections.Generic;
	using MapLayouts;

	/// <summary>
	/// Interface for all layout generator.
	/// </summary>
	/// <typeparam name="TMapDescription">Type of the map description</typeparam>
	/// <typeparam name="TNode">Type of nodes</typeparam>
	public interface ILayoutGenerator<in TMapDescription, TNode>
	{
		/// <summary>
		/// Tries to generate a given number of layouts.
		/// </summary>
		/// <param name="mapDescription">Map description for which layouts are generated.</param>
		/// <param name="numberOfLayouts">How many layouts should be generated.</param>
		/// <returns></returns>
		IList<IMapLayout<TNode>> GetLayouts(TMapDescription mapDescription, int numberOfLayouts);
	}
}