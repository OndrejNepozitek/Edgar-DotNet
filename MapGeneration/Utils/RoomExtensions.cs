using System;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Utils
{
    public static class RoomExtensions
    {
        /// <summary>
        /// Transforms a given point to a position such that if the point was in some relative position to the
        /// RoomShape of the associated MapDescription, the returned point will be in that some relative position
        /// with respect to the position and transformation of the RoomShape in the generated layout.
        /// </summary>
        /// <remarks>
        /// This function is useful e.g. if we have some trap or spawn positions defined for our room shapes and
        /// then need to know where to place them in the generated layout.
        ///
        /// If the room is invariant to some transformation (e.g. does not change when rotated by 90 degrees), then
        /// all such transformations are contained in the room.Transformation collection. All these transformations
        /// can be used for the "transformation" parameter if we do not want to use the room.Transformation transformation.
        /// </remarks>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="room"></param>
        /// <param name="point">Point to be transformed to the new position</param>
        /// <param name="transformation">Transformation to be used. It must be one of the room.Transformations. Defaults to room.Transformation.</param>
        /// <returns></returns>
        public static IntVector2 TransformPointToNewPosition<TNode>(this IRoom<TNode> room, IntVector2 point, Transformation? transformation = null)
        {
            if (transformation != null && room.Transformations.Contains(transformation.Value))
            {
                throw new ArgumentException("Transformation must be one of the possible room transformations", nameof(transformation));
            }

            transformation = transformation ?? room.Transformation;

            var transformedShape = room.RoomDescription.Shape.Transform(transformation.Value);

            return (point.Transform(transformation.Value) - transformedShape.BoundingRectangle.A) + room.Position;
        }
    }
}