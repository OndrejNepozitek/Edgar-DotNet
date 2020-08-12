using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.ConfigParsing
{
    public class RoomTemplateIdentifier
    {
        public string SetName { get; set; }

        public string RoomDescriptionName { get; set; }

        public Vector2Int Scale { get; set; }

        #region Equals

        protected bool Equals(RoomTemplateIdentifier other)
        {
            return SetName == other.SetName && RoomDescriptionName == other.RoomDescriptionName && Scale.Equals(other.Scale);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RoomTemplateIdentifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (SetName != null ? SetName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (RoomDescriptionName != null ? RoomDescriptionName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Scale.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RoomTemplateIdentifier left, RoomTemplateIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoomTemplateIdentifier left, RoomTemplateIdentifier right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}