namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core;
	using Interfaces.Core.Doors;

	public class RoomDescriptionModel
	{
		public string Name { get; set; }

		public List<IntVector2> Shape { get; set; }

		public IDoorMode DoorMode { get; set; }
	}
}