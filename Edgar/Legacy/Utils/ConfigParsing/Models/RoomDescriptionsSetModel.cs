using System.Collections.Generic;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    public class RoomDescriptionsSetModel
	{
		public string Name { get; set; }

		public RoomDescriptionModel Default { get; set; }

		public Dictionary<string, RoomDescriptionModel> RoomDescriptions { get; set; }
	}
}