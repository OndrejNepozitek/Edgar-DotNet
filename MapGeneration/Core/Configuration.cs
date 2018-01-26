namespace MapGeneration.Core
{
	using System;
	using System.Diagnostics.Contracts;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public struct Configuration : IEnergyConfiguration<Configuration, IntAlias<GridPolygon>>
	{
		public IntAlias<GridPolygon> ShapeContainer { get; }

		public GridPolygon Shape => ShapeContainer.Value;

		public IntVector2 Position { get; }

		public SimpleBitVector32 ValidityVector { get; }

		public EnergyData EnergyData { get; }

		public bool IsValid => ValidityVector.Data == 0;

		public Configuration(IntAlias<GridPolygon> shape, IntVector2 position, SimpleBitVector32 validityVector, EnergyData energyData)
		{
			ShapeContainer = shape;
			Position = position;
			ValidityVector = validityVector;
			EnergyData = energyData;
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
			return new Configuration(
				ShapeContainer,
				Position,
				validityVector,
				EnergyData
			);
		}

		public Configuration SetEnergyData(EnergyData energyData)
		{
			return new Configuration(
				ShapeContainer,
				Position,
				ValidityVector,
				energyData
			);
		}
	}
}