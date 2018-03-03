namespace MapGeneration.Interfaces.Core
{
	public interface ILayoutConverter<in TLayoutFrom, out TLayoutTo>
	{
		TLayoutTo Convert(TLayoutFrom layout, bool addDoors);
	}
}