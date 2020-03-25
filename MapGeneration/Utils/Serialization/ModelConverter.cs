using System.Linq;
using MapGeneration.Core.MapLayouts;

namespace MapGeneration.Utils.Serialization
{
    using Models;

	/// <summary>
	/// Converts given class to representations more suitable for serializing.
	/// </summary>
	public class ModelConverter
	{
		public DoorModel<TNode> GetDoorModel<TNode>(DoorInfo<TNode> doorInfo)
		{
			return new DoorModel<TNode>()
			{
				Node = doorInfo.Node,
				From = doorInfo.DoorLine.From,
				To = doorInfo.DoorLine.To,
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