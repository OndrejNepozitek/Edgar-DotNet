namespace MapGeneration.Grid.ConfigSpacesGenerators
{
	using Descriptions;

	public class ExtendedConfigSpacesGenerator
	{
		private ConfigSpacesGenerator configSpacesGenerator = new ConfigSpacesGenerator();

		public ConfigurationSpaces Generate<TNode>(MapDescription<TNode> mapDescription)
		{
			var rooms = mapDescription.GetRooms();
		}
	}
}