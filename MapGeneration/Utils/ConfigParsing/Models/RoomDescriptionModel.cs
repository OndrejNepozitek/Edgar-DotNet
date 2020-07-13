using MapGeneration.Core.Doors.Interfaces;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Utils.ConfigParsing.Models
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

    public class RoomDescriptionModel
	{
		public string Name { get; set; }

		public List<IntVector2> Shape { get; set; }

		public IDoorModeModel DoorMode { get; set; }

        public RepeatMode? RepeatMode { get; set; }
	}
}