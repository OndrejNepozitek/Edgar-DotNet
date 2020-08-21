using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.Utils.Serialization.Models;

namespace Edgar.Legacy.Utils.Serialization
{
    /// <summary>
	/// Converts given class to representations more suitable for serializing.
	/// </summary>
	public class ModelConverter
	{
		public DoorModel<TNode> GetDoorModel<TNode>(LayoutDoorGrid2D<TNode> layoutDoorGrid2D)
		{
			return new DoorModel<TNode>()
			{
				Node = layoutDoorGrid2D.Node,
				From = layoutDoorGrid2D.DoorLine.From,
				To = layoutDoorGrid2D.DoorLine.To,
			};
		}

		public RoomModel<TNode> GetRoomModel<TNode>(Room<TNode> room)
		{
			return new RoomModel<TNode>()
			{
				Node = room.Node,
				Shape = room.Shape.GetPoints(),
				IsCorridor = room.IsCorridor,
				Doors = room.Doors.Select(GetDoorModel).ToList(),
				Position = room.Position
			};
		}
	}
}