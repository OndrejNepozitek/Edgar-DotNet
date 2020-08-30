namespace Edgar.Legacy.Core.LayoutGenerators.Interfaces
{
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