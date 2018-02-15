namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;

	public class RoomModel
	{
		public string Name { get; set; }

		public List<RoomShapesModel> RoomShapes { get; set; }
	}
}