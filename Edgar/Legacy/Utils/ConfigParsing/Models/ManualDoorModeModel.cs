using System.Collections.Generic;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    /// <summary>
	/// Mode that holds all the door positions.
	/// </summary>
	public class ManualDoorModeModel : IDoorModeModel
	{
		public List<OrthogonalLine> DoorPositions { get; set; }
    }
}