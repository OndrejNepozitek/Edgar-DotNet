namespace MapGeneration.Interfaces.Core.LayoutConverters
{
	/// <summary>
	/// Represents type that can convert one representation of a layout to another.
	/// </summary>
	/// <typeparam name="TLayoutFrom"></typeparam>
	/// <typeparam name="TLayoutTo"></typeparam>
	public interface ILayoutConverter<in TLayoutFrom, out TLayoutTo>
	{
		/// <summary>
		/// Converts layout from TLayoutFrom to TLayoutTo.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="addDoors"></param>
		/// <returns></returns>
		TLayoutTo Convert(TLayoutFrom layout, bool addDoors);
	}
}