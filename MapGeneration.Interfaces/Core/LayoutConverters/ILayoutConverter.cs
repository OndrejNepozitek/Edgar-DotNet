namespace MapGeneration.Interfaces.Core.LayoutConverters
{
	public interface ILayoutConverter<in TLayoutFrom, out TLayoutTo>
	{
		TLayoutTo Convert(TLayoutFrom layout, bool addDoors);
	}
}