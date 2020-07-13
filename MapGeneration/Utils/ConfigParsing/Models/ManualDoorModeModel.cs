using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Doors.Interfaces;

namespace MapGeneration.Utils.ConfigParsing.Models
{
    /// <summary>
	/// Mode that holds all the door positions.
	/// </summary>
	public class ManualDoorModeModel : IDoorModeModel
	{
		public List<OrthogonalLine> DoorPositions { get; set; }
    }
}