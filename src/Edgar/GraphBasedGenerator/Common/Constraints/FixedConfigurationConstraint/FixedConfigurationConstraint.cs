using System;
using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint
{
    public class FixedConfigurationConstraint<TRoomShape, TPosition, TRoom> : IFixedConfigurationConstraint<TRoomShape, TPosition, RoomNode<TRoom>>
    {
        private readonly bool hasFixedShapes = false;
        private readonly bool hasFixedPositions = false;

        private readonly ValueWrapper<TRoomShape>?[] fixedShapes;
        private readonly ValueWrapper<TPosition>?[] fixedPositions;

        public FixedConfigurationConstraint(int totalRooms, Dictionary<RoomNode<TRoom>, TRoomShape> fixedShapes, Dictionary<RoomNode<TRoom>, TPosition> fixedPositions)
        {
            if (fixedShapes != null && fixedPositions.Count != 0)
            {
                hasFixedShapes = true;
                this.fixedShapes = new ValueWrapper<TRoomShape>?[totalRooms];

                foreach (var pair in fixedShapes)
                {
                    this.fixedShapes[pair.Key.Id] = new ValueWrapper<TRoomShape>()
                    {
                        Value = pair.Value,
                    };
                }
            }

            if (fixedPositions != null && fixedPositions.Count != 0)
            {
                hasFixedPositions = true;
                this.fixedPositions = new ValueWrapper<TPosition>?[totalRooms];

                foreach (var pair in fixedPositions)
                {
                    this.fixedPositions[pair.Key.Id] = new ValueWrapper<TPosition>()
                    {
                        Value = pair.Value,
                    };
                }
            }
        }

        public bool IsFixedShape(RoomNode<TRoom> room)
        {
            return TryGetFixedShape(room, out _);
        }

        public TRoomShape GetFixeShape(RoomNode<TRoom> room)
        {
            if (!TryGetFixedShape(room, out var shape))
            {
                throw new InvalidOperationException("The shape of the room is not fixed!");
            }

            return shape;
        }

        public bool TryGetFixedShape(RoomNode<TRoom> room, out TRoomShape shape)
        {
            shape = default(TRoomShape);

            if (!hasFixedShapes)
            {
                return false;
            }

            var fixedShape = fixedShapes[room.Id];

            if (fixedShape.HasValue)
            {
                shape = fixedShape.Value.Value;
                return true;
            }

            return false;
        }

        public bool IsFixedPosition(RoomNode<TRoom> room)
        {
            return TryGetFixedPosition(room, out _);
        }

        public TPosition GetFixedPosition(RoomNode<TRoom> room)
        {
            if (!TryGetFixedPosition(room, out var position))
            {
                throw new InvalidOperationException("The position of the room is not fixed!");
            }

            return position;
        }

        public bool TryGetFixedPosition(RoomNode<TRoom> room, out TPosition position)
        {
            position = default(TPosition);

            if (!hasFixedPositions)
            {
                return false;
            }

            var fixedPosition = fixedPositions[room.Id];

            if (fixedPosition.HasValue)
            {
                position = fixedPosition.Value.Value;
                return true;
            }

            return false;
        }

        private struct ValueWrapper<TValue>
        {
            public TValue Value;
        }
    }
}