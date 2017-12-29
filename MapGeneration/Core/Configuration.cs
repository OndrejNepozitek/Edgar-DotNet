namespace MapGeneration.Core
{
	using System;
	using System.Diagnostics.Contracts;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public struct Configuration<TShape> : IConfiguration<Configuration<TShape>, TShape>
	{
		public TShape Shape { get; }

		public IntVector2 Position { get; }

		public readonly SimpleBitVector32 ValidityVector;

		public Configuration(TShape shape, IntVector2 position, SimpleBitVector32 validityVector)
		{
			Shape = shape;
			Position = position;
			ValidityVector = validityVector;
		}

		[Pure]
		public Configuration<TShape> SetShape(TShape shape)
		{
			throw new InvalidOperationException();
		}

		[Pure]
		public Configuration<TShape> SetPosition(IntVector2 position)
		{
			throw new InvalidOperationException();
		}

		[Pure]
		public Configuration<TShape> SetValidityVector(SimpleBitVector32 validityVector)
		{
			throw new InvalidOperationException();
		}
	}
}