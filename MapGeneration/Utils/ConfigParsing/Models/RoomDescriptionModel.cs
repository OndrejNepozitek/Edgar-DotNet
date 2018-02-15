namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;
	using Core.Interfaces;
	using GeneralAlgorithms.DataStructures.Common;

	public class RoomDescriptionModel
	{
		public string Name { get; set; }

		public List<IntVector2> Shape { get; set; }

		public IDoorMode DoorMode { get; set; }
	}
}