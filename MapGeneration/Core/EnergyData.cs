namespace MapGeneration.Core
{
	using System;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;

	public struct EnergyData : IValidityVectorEnergyData<EnergyData>
	{
		public float Energy { get; }

		public int Overlap { get; }

		// We need an int here because there must be no rounding of the value
		public int MoveDistance { get; }

		public bool IsValid => throw new NotSupportedException();

		public SimpleBitVector32 ValidityVector { get; }

		public EnergyData(float energy, int area, int moveDistance, SimpleBitVector32 validityVector)
		{
			Energy = energy;
			Overlap = area;
			MoveDistance = moveDistance;
			ValidityVector = validityVector;
		}

		public EnergyData SetEnergy(float energy)
		{
			return new EnergyData(
				energy,
				Overlap,
				MoveDistance,
				ValidityVector
			);
		}

		public EnergyData SetOverlap(int area)
		{
			return new EnergyData(
				Energy,
				area,
				MoveDistance,
				ValidityVector
			);
		}

		public EnergyData SetMoveDistance(int moveDistance)
		{
			return new EnergyData(
				Energy,
				Overlap,
				moveDistance,
				ValidityVector
			);
		}

		public EnergyData SetIsValid(bool isValid)
		{
			throw new NotImplementedException();
		}

		public EnergyData SetValidityVector(SimpleBitVector32 validityVector)
		{
			return new EnergyData(
				Energy,
				Overlap,
				MoveDistance,
				validityVector
			);
		}
	}
}