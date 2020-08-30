using System.Collections.Generic;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    public class CorridorsModel
	{
		public bool? Enable { get; set; }

		public List<int> Offsets { get; set; }

		public List<RoomShapesModel> CorridorShapes { get; set; }
	}
}