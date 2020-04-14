using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.EnergyData;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Simplified
{
    public class SimpleConfiguration<TRoom> : IEnergyConfiguration<RoomTemplateInstance, TRoom, CorridorsData>
    {
        public TRoom Room { get; set; }

        public IntVector2 Position { get; set; }
        
        public RoomTemplateInstance RoomTemplateInstance { get; set; }

        public CorridorsData EnergyData { get; set; }

        // TODO: handle better
        #region IConfiguration interface
        
        GridPolygon IConfiguration<RoomTemplateInstance, TRoom>.Shape => RoomTemplateInstance.RoomShape;

        RoomTemplateInstance IConfiguration<RoomTemplateInstance, TRoom>.ShapeContainer => RoomTemplateInstance;

        TRoom IConfiguration<RoomTemplateInstance, TRoom>.Node => Room;

        RoomTemplateInstance IMutableConfiguration<RoomTemplateInstance, TRoom>.ShapeContainer
        {
            get => RoomTemplateInstance;
            set => RoomTemplateInstance = value;
        }

        TRoom IMutableConfiguration<RoomTemplateInstance, TRoom>.Node
        {
            get => Room;
            set => Room = value;
        }

        #endregion

        public SimpleConfiguration(TRoom room, IntVector2 position, RoomTemplateInstance roomTemplateInstance)
        {
            Room = room;
            Position = position;
            RoomTemplateInstance = roomTemplateInstance;
        }
    }
}