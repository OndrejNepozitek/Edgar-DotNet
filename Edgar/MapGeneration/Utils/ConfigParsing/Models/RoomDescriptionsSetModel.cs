namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;

	public class RoomDescriptionsSetModel
	{
		public string Name { get; set; }

		public RoomDescriptionModel Default { get; set; }

		public Dictionary<string, RoomDescriptionModel> RoomDescriptions { get; set; }
	}
}