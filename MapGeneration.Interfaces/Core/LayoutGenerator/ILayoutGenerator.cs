namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System.Collections.Generic;
	using MapLayouts;

	/// <summary>
	/// Interface for all layout generator.
	/// </summary>
	/// <typeparam name="TMapDescription">Type of the map description</typeparam>
	/// <typeparam name="TLayout">Type of the layout that is returned</typeparam>
	public interface ILayoutGenerator<in TMapDescription, TLayout>
	{
		/// <summary>
		/// Tries to generate a given number of layouts.
		/// </summary>
		/// <param name="mapDescription">Map description for which layouts are generated.</param>
		/// <param name="numberOfLayouts">How many layouts should be generated.</param>
		/// <returns></returns>
		IList<TLayout> GetLayouts(TMapDescription mapDescription, int numberOfLayouts);
	}
}