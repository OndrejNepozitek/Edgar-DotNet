using Edgar.GraphBasedGenerator.Grid2D.Internal;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
    public class ConfigurationSpaceSelector
    {
        public RoomTemplateInstanceGrid2D MovingRoomTemplateInstance { get; }

        public RoomTemplateInstanceGrid2D FixedRoomTemplateInstance { get; }

        public RoomTemplateInstanceGrid2D CorridorRoomTemplateInstance { get; }

        public ConfigurationSpaceSelector(RoomTemplateInstanceGrid2D movingRoomTemplateInstance, RoomTemplateInstanceGrid2D fixedRoomTemplateInstance, RoomTemplateInstanceGrid2D corridorRoomTemplateInstance)
        {
            MovingRoomTemplateInstance = movingRoomTemplateInstance;
            FixedRoomTemplateInstance = fixedRoomTemplateInstance;
            CorridorRoomTemplateInstance = corridorRoomTemplateInstance;
        }

        protected bool Equals(ConfigurationSpaceSelector other)
        {
            return Equals(MovingRoomTemplateInstance, other.MovingRoomTemplateInstance) && Equals(FixedRoomTemplateInstance, other.FixedRoomTemplateInstance) && Equals(CorridorRoomTemplateInstance, other.CorridorRoomTemplateInstance);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConfigurationSpaceSelector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MovingRoomTemplateInstance != null ? MovingRoomTemplateInstance.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FixedRoomTemplateInstance != null ? FixedRoomTemplateInstance.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CorridorRoomTemplateInstance != null ? CorridorRoomTemplateInstance.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ConfigurationSpaceSelector left, ConfigurationSpaceSelector right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConfigurationSpaceSelector left, ConfigurationSpaceSelector right)
        {
            return !Equals(left, right);
        }
    }
}