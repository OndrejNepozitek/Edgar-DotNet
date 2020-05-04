using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Simplified.ConfigurationSpaces
{
    public class ConfigurationSpaceSelector
    {
        public RoomTemplateInstance MovingRoomTemplateInstance { get; }

        public RoomTemplateInstance FixedRoomTemplateInstance { get; }

        public RoomTemplateInstance CorridorRoomTemplateInstance { get; }

        public ConfigurationSpaceSelector(RoomTemplateInstance movingRoomTemplateInstance, RoomTemplateInstance fixedRoomTemplateInstance, RoomTemplateInstance corridorRoomTemplateInstance)
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
            return Equals((ConfigurationSpaceSelector) obj);
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