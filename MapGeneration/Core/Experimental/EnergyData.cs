namespace MapGeneration.Core.Experimental
{
	using System;
	using Interfaces;

	public struct EnergyData : IEnergyData<EnergyData>
	{
		public float Energy { get; }

		public int Overlap { get; }

		// We need an int here because there must be no rounding of the value
		public int MoveDistance { get; }

		public bool IsValid { get; }

		public EnergyData(float energy, int area, int moveDistance, bool isValid)
		{
			Energy = energy;
			Overlap = area;
			MoveDistance = moveDistance;
			IsValid = isValid;
		}

		public EnergyData SetEnergy(float energy)
		{
			return new EnergyData(
				energy,
				Overlap,
				MoveDistance,
				IsValid
			);
		}

		public EnergyData SetOverlap(int area)
		{
			return new EnergyData(
				Energy,
				area,
				MoveDistance,
				IsValid
			);
		}

		public EnergyData SetMoveDistance(int moveDistance)
		{
			return new EnergyData(
				Energy,
				Overlap,
				moveDistance,
				IsValid
			);
		}

		public EnergyData SetIsValid(bool isValid)
		{
			return new EnergyData(
				Energy,
				Overlap,
				MoveDistance,
				isValid
			);
		}
	}
}