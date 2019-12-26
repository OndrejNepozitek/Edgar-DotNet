namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System.Collections.Generic;

    /// <summary>
	/// Interface for all layout generators.
	/// </summary>
    /// <typeparam name="TLayout">Type of the layout that is generated</typeparam>
	public interface ILayoutGenerator<out TLayout>
	{
		/// <summary>
		/// Generates a layout.
		/// </summary>
        /// <returns></returns>
		TLayout GenerateLayout();
	}
}