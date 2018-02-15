namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System;
	using System.Collections.Generic;

	public class MapDescriptionModel
	{
		public List<RoomShapesModel> DefaultRoomShapes { get; set; }

		public List<RoomModel> Rooms { get; set; }

		public List<Tuple<string, string>> Passages { get; set; }

		public RoomDescriptionsSetModel CustomRoomDescriptionsSet { get; set; }
	}
}