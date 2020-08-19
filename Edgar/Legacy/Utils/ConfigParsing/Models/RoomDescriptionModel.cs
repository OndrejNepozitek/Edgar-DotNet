using System.Collections.Generic;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    public class RoomDescriptionModel
	{
		public string Name { get; set; }

		public List<Vector2Int> Shape { get; set; }

		public IDoorModeModel DoorMode { get; set; }

        public RoomTemplateRepeatMode? RepeatMode { get; set; }
	}
}