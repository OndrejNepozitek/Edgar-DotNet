namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

	public class MapDescriptionModel
	{
		public List<RoomShapesModel> DefaultRoomShapes { get; set; }

		public RoomsRangeModel RoomsRange { get; set; }

		public Dictionary<List<int>, RoomModel> Rooms { get; set; }

		public List<IntVector2> Passages { get; set; }

		public RoomDescriptionsSetModel CustomRoomDescriptionsSet { get; set; }

		public CorridorsModel Corridors { get; set; }
	}
}