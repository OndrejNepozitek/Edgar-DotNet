using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Common.Exceptions
{
    public class NoSuitableShapeForRoomException : GeneratorException
    {
        public object Room { get; }

        public List<object> NeighboringShapes { get; }

        public NoSuitableShapeForRoomException(string message, object room, List<object> neighboringShapes = null) : base(message)
        {
            Room = room;
            NeighboringShapes = neighboringShapes;
        }
    }
}