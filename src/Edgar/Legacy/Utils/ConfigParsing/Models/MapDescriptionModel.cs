using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    public class MapDescriptionModel
    {
        public List<RoomShapesModel> DefaultRoomShapes { get; set; }

        public RoomsRangeModel RoomsRange { get; set; }

        public Dictionary<List<int>, RoomModel> Rooms { get; set; }

        public List<Vector2Int> Passages { get; set; }

        public RoomDescriptionsSetModel CustomRoomDescriptionsSet { get; set; }

        public CorridorsModel Corridors { get; set; }
    }
}