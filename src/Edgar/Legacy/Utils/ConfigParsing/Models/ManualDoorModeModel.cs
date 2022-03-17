using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    /// <summary>
    /// Mode that holds all the door positions.
    /// </summary>
    public class ManualDoorModeModel : IDoorModeModel
    {
        public List<OrthogonalLineGrid2D> DoorPositions { get; set; }
    }
}