namespace MapGeneration.Core
{
	using System;
	using System.Diagnostics.Contracts;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public struct Configuration : IConfiguration<Configuration, IntAlias<GridPolygon>>
	{
		public IntAlias<GridPolygon> ShapeContainer { get; }

		public GridPolygon Shape => ShapeContainer.Value;

		public IntVector2 Position { get; }

		public SimpleBitVector32 ValidityVector { get; }

		public Configuration(IntAlias<GridPolygon> shape, IntVector2 position, SimpleBitVector32 validityVector)
		{
			ShapeContainer = shape;
			Position = position;
			ValidityVector = validityVector;
		}

		[Pure]
		public Configuration SetShape(IntAlias<GridPolygon> shape)
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