namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;

	public class CorridorsModel
	{
		public bool? Enable { get; set; }

		public List<int> Offsets { get; set; }

		public List<RoomShapesModel> CorridorShapes { get; set; }
	}
}