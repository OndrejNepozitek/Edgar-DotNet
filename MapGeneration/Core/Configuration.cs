namespace MapGeneration.Core
{
	using System;
	using System.Diagnostics.Contracts;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public struct Configuration
	{
		public readonly GridPolygon Shape;

		public readonly IntVector2 Position;

		public readonly SimpleBitVector32 ValidityVector;

		public Configuration(GridPolygon shape, IntVector2 position, SimpleBitVector32 validityVector)
		{
			Shape = shape;
			Position = position;
			ValidityVector = validityVector;
		}

		[Pure]
		public Configuration SetShape(GridPolygon shape)
		{
			throw new InvalidOperationException();
		}

		[Pure]
		public Configuration SetPosition(IntVector2 position)
		{
			throw new InvalidOperationException();
		}

		[Pure]
		public Configuration SetValidityVector(SimpleBitVector32 validityVector)
		{
			throw new InvalidOperationException();
		}
	}
}