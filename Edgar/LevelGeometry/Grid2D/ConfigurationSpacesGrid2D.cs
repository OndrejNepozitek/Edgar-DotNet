using System.Collections.Generic;

namespace Edgar.LevelGeometry.Grid2D
{
    public class ConfigurationSpacesGrid2D
    {
        private readonly Dictionary<Selector, ConfigurationSpaceGrid2D> configurationSpaces = new Dictionary<Selector, ConfigurationSpaceGrid2D>();

        public ConfigurationSpaceGrid2D GetConfigurationSpace(RoomShapeGrid2D movingRoom, RoomShapeGrid2D fixedRoom, Options options)
        {
            var selector = new Selector(movingRoom, fixedRoom, options);

            if (configurationSpaces.TryGetValue(selector, out var configurationSpace))
            {
                return configurationSpace;
            }

            configurationSpace = null;
        }

        public ConfigurationSpaceGrid2D GetMaximumIntersection(RoomShapeGrid2D movingRoom, List<Configuration> fixedRooms)
        {
            return null;
        }

        public class Options
        {
            // Should we use "doors"? Or connection points?
            public int DoorsDistance { get; }

            // Should we use "corridors"?
            public IReadOnlyList<RoomShapeGrid2D> Corridors { get; }

            public Options(IReadOnlyList<RoomShapeGrid2D> corridors = null, int doorsDistance = 0)
            {
                Corridors = corridors;
                DoorsDistance = doorsDistance;
            }
        }

        public class Configuration
        {
           public RoomShapeGrid2D RoomShape { get; set; }

           public Vector2Int Position { get; set; }

           public Options Options { get; set; }
        }

        private class Selector
        {
            private RoomShapeGrid2D MovingRoom { get; }

            private RoomShapeGrid2D FixedRoom { get; }

            private Options Options { get; }

            public Selector(RoomShapeGrid2D movingRoom, RoomShapeGrid2D fixedRoom, Options options)
            {
                MovingRoom = movingRoom;
                FixedRoom = fixedRoom;
                Options = options;
            }

            #region Equals

            protected bool Equals(Selector other)
            {
                return MovingRoom.Equals(other.MovingRoom) && FixedRoom.Equals(other.FixedRoom) && Options.Equals(other.Options);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Selector) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = MovingRoom.GetHashCode();
                    hashCode = (hashCode * 397) ^ FixedRoom.GetHashCode();
                    hashCode = (hashCode * 397) ^ Options.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(Selector left, Selector right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Selector left, Selector right)
            {
                return !Equals(left, right);
            }

            #endregion
        }
    }
}