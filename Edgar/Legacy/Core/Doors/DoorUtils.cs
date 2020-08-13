using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.Doors
{
    public static class DoorUtils
	{
		/// <summary>
		/// Merges all door lines that are directly next to each other and have the same length.
		/// </summary>
		/// <param name="doorLines"></param>
		/// <returns></returns>
		public static List<DoorLine> MergeDoorLines(IEnumerable<DoorLine> doorLines)
		{
			var doorLinesByDirection = doorLines.GroupBy(x => x.Line.GetDirection());
			var result = new List<DoorLine>();

			foreach (var grouping in doorLinesByDirection)
			{
				if (grouping.Key == OrthogonalLine.Direction.Undefined)
					throw new ArgumentException("There must be no door lines with undefined direction");

				var sameDirectionDoorLines = new LinkedList<DoorLine>(grouping);

				while (sameDirectionDoorLines.Count != 0)
				{
					var doorLineNode = sameDirectionDoorLines.First;
					var doorLine = doorLineNode.Value;
					sameDirectionDoorLines.RemoveFirst();

					while (true)
					{
						var found = false;
						var nextDoorLineNode = sameDirectionDoorLines.First;

						while (nextDoorLineNode != null)
						{
							var otherDoorLineNode = nextDoorLineNode;
							var otherDoorLine = otherDoorLineNode.Value;
							nextDoorLineNode = otherDoorLineNode.Next;

							if (otherDoorLine.Length != doorLine.Length)
							{
								continue;
							}
								
							if (doorLine.Line.To + doorLine.Line.GetDirectionVector() == otherDoorLine.Line.From)
							{
								doorLine = new DoorLine(new OrthogonalLine(doorLine.Line.From, otherDoorLine.Line.To), doorLine.Length);
								found = true;
								sameDirectionDoorLines.Remove(otherDoorLineNode);
							}
							else if (doorLine.Line.From - doorLine.Line.GetDirectionVector() == otherDoorLine.Line.To)
							{
								doorLine = new DoorLine(new OrthogonalLine(otherDoorLine.Line.From, doorLine.Line.To), doorLine.Length);
								found = true;
								sameDirectionDoorLines.Remove(otherDoorLineNode);
							}
						}

						if (!found)
							break;
					}

					result.Add(doorLine);
				}
			}

			return result;
		}

        public static List<DoorLineGrid2D> MergeDoorLines(IEnumerable<DoorLineGrid2D> doorLines)
        {
            var doorLinesByDirection = doorLines.GroupBy(x => x.Line.GetDirection());
            var result = new List<DoorLineGrid2D>();

            foreach (var grouping in doorLinesByDirection)
            {
                if (grouping.Key == OrthogonalLine.Direction.Undefined)
                    throw new ArgumentException("There must be no door lines with undefined direction");

                var sameDirectionDoorLines = new LinkedList<DoorLineGrid2D>(grouping);

                while (sameDirectionDoorLines.Count != 0)
                {
                    var doorLineNode = sameDirectionDoorLines.First;
                    var doorLine = doorLineNode.Value;
                    sameDirectionDoorLines.RemoveFirst();

                    while (true)
                    {
                        var found = false;
                        var nextDoorLineNode = sameDirectionDoorLines.First;

                        while (nextDoorLineNode != null)
                        {
                            var otherDoorLineNode = nextDoorLineNode;
                            var otherDoorLine = otherDoorLineNode.Value;
                            nextDoorLineNode = otherDoorLineNode.Next;

                            if (otherDoorLine.Length != doorLine.Length)
                            {
                                continue;
                            }

							// TODO: improve later
                            if (otherDoorLine.DoorSocket != doorLine.DoorSocket)
                            {
                                continue;
                            }
								
                            if (doorLine.Line.To + doorLine.Line.GetDirectionVector() == otherDoorLine.Line.From)
                            {
                                doorLine = new DoorLineGrid2D(new OrthogonalLine(doorLine.Line.From, otherDoorLine.Line.To), doorLine.Length, doorLine.DoorSocket);
                                found = true;
                                sameDirectionDoorLines.Remove(otherDoorLineNode);
                            }
                            else if (doorLine.Line.From - doorLine.Line.GetDirectionVector() == otherDoorLine.Line.To)
                            {
                                doorLine = new DoorLineGrid2D(new OrthogonalLine(otherDoorLine.Line.From, doorLine.Line.To), doorLine.Length, doorLine.DoorSocket);
                                found = true;
                                sameDirectionDoorLines.Remove(otherDoorLineNode);
                            }
                        }

                        if (!found)
                            break;
                    }

                    result.Add(doorLine);
                }
            }

            return result;
        }

		/// <summary>
		/// Transform door line according to a given transformation.
		/// </summary>
		/// <param name="doorLine"></param>
		/// <param name="transformation"></param>
		/// <returns></returns>
		public static DoorLine TransformDoorLine(DoorLine doorLine, Transformation transformation)
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

        public static DoorLineGrid2D TransformDoorLine(DoorLineGrid2D doorLine, Transformation transformation)
        {
            var doorPosition = doorLine.Line;

            if (doorPosition.GetDirection() == OrthogonalLine.Direction.Undefined)
                throw new InvalidOperationException("Cannot fix door direction when original direction is undefined");

            switch (transformation)
            {
                case Transformation.Identity:
                    return doorLine;

                case Transformation.Rotate90:
                    return new DoorLineGrid2D(doorPosition.Rotate(90), doorLine.Length, doorLine.DoorSocket);

                case Transformation.Rotate180:
                    return new DoorLineGrid2D(doorPosition.Rotate(180), doorLine.Length, doorLine.DoorSocket);

                case Transformation.Rotate270:
                    return new DoorLineGrid2D(doorPosition.Rotate(270), doorLine.Length, doorLine.DoorSocket);
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

            return new DoorLineGrid2D(newDoorPosition, doorLine.Length, doorLine.DoorSocket);
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