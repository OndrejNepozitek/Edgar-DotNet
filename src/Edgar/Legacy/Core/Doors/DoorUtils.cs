using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
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
				if (grouping.Key == OrthogonalLineGrid2D.Direction.Undefined)
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
								doorLine = new DoorLine(new OrthogonalLineGrid2D(doorLine.Line.From, otherDoorLine.Line.To), doorLine.Length);
								found = true;
								sameDirectionDoorLines.Remove(otherDoorLineNode);
							}
							else if (doorLine.Line.From - doorLine.Line.GetDirectionVector() == otherDoorLine.Line.To)
							{
								doorLine = new DoorLine(new OrthogonalLineGrid2D(otherDoorLine.Line.From, doorLine.Line.To), doorLine.Length);
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
                if (grouping.Key == OrthogonalLineGrid2D.Direction.Undefined)
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
                                doorLine = new DoorLineGrid2D(new OrthogonalLineGrid2D(doorLine.Line.From, otherDoorLine.Line.To), doorLine.Length, doorLine.DoorSocket);
                                found = true;
                                sameDirectionDoorLines.Remove(otherDoorLineNode);
                            }
                            else if (doorLine.Line.From - doorLine.Line.GetDirectionVector() == otherDoorLine.Line.To)
                            {
                                doorLine = new DoorLineGrid2D(new OrthogonalLineGrid2D(otherDoorLine.Line.From, doorLine.Line.To), doorLine.Length, doorLine.DoorSocket);
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
		public static DoorLine TransformDoorLine(DoorLine doorLine, TransformationGrid2D transformation)
		{
			var doorPosition = doorLine.Line;

			if (doorPosition.GetDirection() == OrthogonalLineGrid2D.Direction.Undefined)
				throw new InvalidOperationException("Cannot fix door direction when original direction is undefined");

			switch (transformation)
			{
				case TransformationGrid2D.Identity:
					return doorLine;

				case TransformationGrid2D.Rotate90:
					return new DoorLine(doorPosition.Rotate(90), doorLine.Length);

				case TransformationGrid2D.Rotate180:
					return new DoorLine(doorPosition.Rotate(180), doorLine.Length);

				case TransformationGrid2D.Rotate270:
					return new DoorLine(doorPosition.Rotate(270), doorLine.Length);
			}

			// Other transformations need to switch door directions
			var firstStartPoint = doorPosition.From.Transform(transformation);
			var lastStartPoint = doorPosition.To.Transform(transformation);
			var length = doorLine.Length;
			var transformedDirection = TransformDirection(doorPosition.GetDirection(), transformation);
			var transformedLine = new OrthogonalLineGrid2D(firstStartPoint, lastStartPoint, transformedDirection);

			var lastEndPoint = lastStartPoint + length * transformedLine.GetDirectionVector();

			var newDirection = OrthogonalLineGrid2D.GetOppositeDirection(transformedDirection);
			var newDoorPosition = new OrthogonalLineGrid2D(lastEndPoint, lastEndPoint + transformedLine. Length * transformedLine.SwitchOrientation().GetDirectionVector(), newDirection);

			if (newDoorPosition.Length != doorPosition.Length)
			{
				throw new InvalidOperationException();
			}

			return new DoorLine(newDoorPosition, doorLine.Length);
		}

        public static DoorLineGrid2D TransformDoorLine(DoorLineGrid2D doorLine, TransformationGrid2D transformation)
        {
            var doorPosition = doorLine.Line;

            if (doorPosition.GetDirection() == OrthogonalLineGrid2D.Direction.Undefined)
                throw new InvalidOperationException("Cannot fix door direction when original direction is undefined");

            switch (transformation)
            {
                case TransformationGrid2D.Identity:
                    return doorLine;

                case TransformationGrid2D.Rotate90:
                    return new DoorLineGrid2D(doorPosition.Rotate(90), doorLine.Length, doorLine.DoorSocket);

                case TransformationGrid2D.Rotate180:
                    return new DoorLineGrid2D(doorPosition.Rotate(180), doorLine.Length, doorLine.DoorSocket);

                case TransformationGrid2D.Rotate270:
                    return new DoorLineGrid2D(doorPosition.Rotate(270), doorLine.Length, doorLine.DoorSocket);
            }

            // Other transformations need to switch door directions
            var firstStartPoint = doorPosition.From.Transform(transformation);
            var lastStartPoint = doorPosition.To.Transform(transformation);
            var length = doorLine.Length;
            var transformedDirection = TransformDirection(doorPosition.GetDirection(), transformation);
            var transformedLine = new OrthogonalLineGrid2D(firstStartPoint, lastStartPoint, transformedDirection);

            var lastEndPoint = lastStartPoint + length * transformedLine.GetDirectionVector();

            var newDirection = OrthogonalLineGrid2D.GetOppositeDirection(transformedDirection);
            var newDoorPosition = new OrthogonalLineGrid2D(lastEndPoint, lastEndPoint + transformedLine. Length * transformedLine.SwitchOrientation().GetDirectionVector(), newDirection);

            if (newDoorPosition.Length != doorPosition.Length)
            {
                throw new InvalidOperationException();
            }

            return new DoorLineGrid2D(newDoorPosition, doorLine.Length, doorLine.DoorSocket);
        }

		private static OrthogonalLineGrid2D.Direction TransformDirection(OrthogonalLineGrid2D.Direction originalDirection, TransformationGrid2D transformation)
		{
			if (originalDirection == OrthogonalLineGrid2D.Direction.Undefined)
				throw new InvalidOperationException("Cannot transform direction when original direction is undefined");

			switch (transformation)
			{
				case TransformationGrid2D.MirrorX:
					if (IsHorizontal(originalDirection))
					{
						return originalDirection;
					}

					return OrthogonalLineGrid2D.GetOppositeDirection(originalDirection);

				case TransformationGrid2D.MirrorY:
					if (IsHorizontal(originalDirection))
					{
						return OrthogonalLineGrid2D.GetOppositeDirection(originalDirection);
					}

					return originalDirection;

				case TransformationGrid2D.Diagonal13:
					switch (originalDirection)
					{
						case OrthogonalLineGrid2D.Direction.Top:
							return OrthogonalLineGrid2D.Direction.Right;

						case OrthogonalLineGrid2D.Direction.Right:
							return OrthogonalLineGrid2D.Direction.Top;

						case OrthogonalLineGrid2D.Direction.Bottom:
							return OrthogonalLineGrid2D.Direction.Left;

						case OrthogonalLineGrid2D.Direction.Left:
							return OrthogonalLineGrid2D.Direction.Bottom;

						default:
							throw new ArgumentException();
					}

				case TransformationGrid2D.Diagonal24:
					switch (originalDirection)
					{
						case OrthogonalLineGrid2D.Direction.Top:
							return OrthogonalLineGrid2D.Direction.Left;

						case OrthogonalLineGrid2D.Direction.Right:
							return OrthogonalLineGrid2D.Direction.Bottom;

						case OrthogonalLineGrid2D.Direction.Bottom:
							return OrthogonalLineGrid2D.Direction.Right;

						case OrthogonalLineGrid2D.Direction.Left:
							return OrthogonalLineGrid2D.Direction.Top;

						default:
							throw new ArgumentException();
					}
			}

			throw new ArgumentException();
		}

		private static bool IsHorizontal(OrthogonalLineGrid2D.Direction direction)
		{
			if (direction == OrthogonalLineGrid2D.Direction.Left || direction == OrthogonalLineGrid2D.Direction.Right)
			{
				return true;
			}

			return false;
		}
	}
}