using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.EnergyData;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Simplified
{
    public class SimpleConfiguration<TRoom> : IConfiguration<RoomTemplateInstance, TRoom>
    {
        public TRoom Room { get; }

        public IntVector2 Position { get; }
        
        public RoomTemplateInstance RoomTemplateInstance { get; }

        // TODO: handle better
        #region IConfiguration interface
        
        GridPolygon IConfiguration<RoomTemplateInstance, TRoom>.Shape => RoomTemplateInstance.RoomShape;

        RoomTemplateInstance IShapeConfiguration<RoomTemplateInstance>.ShapeContainer => RoomTemplateInstance;

        TRoom IConfiguration<RoomTemplateInstance, TRoom>.Node => Room;

        #endregion

        public SimpleConfiguration(TRoom room, IntVector2 position, RoomTemplateInstance roomTemplateInstance)
        {
            Room = room;
            Position = position;
            RoomTemplateInstance = roomTemplateInstance;
        }
    }
}