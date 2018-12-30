namespace MapGeneration.Core.Doors
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Doors;

	public static class DoorUtils
	{
		//public static List<IDoorLine> FixDoorDirections(GridPolygon polygon, IEnumerable<IDoorLine> doorLines)
		//{
		//	var polygonLines = polygon.GetLines();
		//}

		//private static DoorLine FixDoorDirection(List<OrthogonalLine> polygonLines, IDoorLine doorLine)
		//{
		//	if (doorLine.Length == 0)
		//		throw new InvalidOperationException();

		//	var doorPosition = doorLine.Line;

		//	if (doorPosition.GetDirection() == OrthogonalLine.Direction.Undefined)
		//		throw new InvalidOperationException("Cannot fix door direction when original direction is undefined");

		//	foreach (var side in polygonLines)
		//	{
		//		if (side.Contains(doorPosition.From) == -1 || side.Contains(doorPosition.To) == -1)
		//			continue;

		//		var isGoodDirection = doorPosition.From + doorPosition.Length * side.GetDirectionVector() == doorPosition.To;



		//		var from = isGoodDirection ? doorPosition.From : doorPosition.To;

		//		return new DoorLine(new OrthogonalLine(from, from, side.GetDirection()), doorPosition.Length);
		//	}

		//	throw new InvalidOperationException("Given door position is not on any side of the polygon");
		//}

		public static IDoorLine TransformDoorLine(IDoorLine doorLine, Transformation transformation)
		{
			var doorPosition = doorLine.Line;

			if (doorPosition.GetDirection() == OrthogonalLine.Direction.Undefined)
				throw new InvalidOperationException("Cannot fix door direction when original direction is undefined");

			switch (transformation)
			{
				case Transformation.Identity:
					return doorLine;

				case Transformation.Rotate90:
					return new DoorLine(doorPosition.Rotate(90), doorLine.Length);

				case Transformation.Rotate180:
					return new DoorLine(doorPosition.Rotate(180), doorLine.Length);

				case Transformation.Rotate270:
					return new DoorLine(doorPosition.Rotate(270), doorLine.Length);
			}

			// Other transformations need to switch door directions
			var firstStartPoint = doorPosition.From.Transform(transformation);
			var lastStartPoint = doorPosition.To.Transform(transformation);
			var length = doorLine.Length;
			var transformedDirection = TransformDirection(doorPosition.GetDirection(), transformation);
			var transformedLine = new OrthogonalLine(firstStartPoint, lastStartPoint, transformedDirection);

			var lastEndPoint = lastStartPoint + length * transformedLine.GetDirectionVector();

			var newDirection = OrthogonalLine.GetOppositeDirection(transformedDirection);
			var newDoorPosition = new OrthogonalLine(lastEndPoint, lastEndPoint + transformedLine. Length * transformedLine.SwitchOrientation().GetDirectionVector(), newDirection);

			if (newDoorPosition.Length != doorPosition.Length)
			{
				throw new InvalidOperationException();
			}

			return new DoorLine(newDoorPosition, doorLine.Length);
		}

		private static OrthogonalLine.Direction TransformDirection(OrthogonalLine.Direction originalDirection, Transformation transformation)
		{
			if (originalDirection == OrthogonalLine.Direction.Undefined)
				throw new InvalidOperationException("Cannot transform direction when original direction is undefined");

			switch (transformation)
			{
				case Transformation.MirrorX:
					if (IsHorizontal(originalDirection))
					{
						return originalDirection;
					}

					return OrthogonalLine.GetOppositeDirection(originalDirection);

				case Transformation.MirrorY:
					if (IsHorizontal(originalDirection))
					{
						return OrthogonalLine.GetOppositeDirection(originalDirection);
					}

					return originalDirection;

				case Transformation.Diagonal13:
					switch (originalDirection)
					{
						case OrthogonalLine.Direction.Top:
							return OrthogonalLine.Direction.Right;

						case OrthogonalLine.Direction.Right:
							return OrthogonalLine.Direction.Top;

						case OrthogonalLine.Direction.Bottom:
							return OrthogonalLine.Direction.Left;

						case OrthogonalLine.Direction.Left:
							return OrthogonalLine.Direction.Bottom;

						default:
							throw new ArgumentException();
					}

				case Transformation.Diagonal24:
					switch (originalDirection)
					{
						case OrthogonalLine.Direction.Top:
							return OrthogonalLine.Direction.Left;

						case OrthogonalLine.Direction.Right:
							return OrthogonalLine.Direction.Bottom;

						case OrthogonalLine.Direction.Bottom:
							return OrthogonalLine.Direction.Right;

						case OrthogonalLine.Direction.Left:
							return OrthogonalLine.Direction.Top;

						default:
							throw new ArgumentException();
					}
			}

			throw new ArgumentException();
		}

		private static bool IsHorizontal(OrthogonalLine.Direction direction)
		{
			if (direction == OrthogonalLine.Direction.Left || direction == OrthogonalLine.Direction.Right)
			{
				return true;
			}

			return false;
		}
	}
}