namespace MapGeneration.Core
{
	using System;
	using Interfaces;

	public struct EnergyData : IEnergyData<EnergyData>
	{
		public float Energy { get; }

		public int Overlap { get; }

		// We need an int here because there must be no rounding of the value
		public int MoveDistance { get; }

		public bool IsValid => throw new NotSupportedException();

		public EnergyData(float energy, int area, int moveDistance)
		{
			Energy = energy;
			Overlap = area;
			MoveDistance = moveDistance;
		}

		public EnergyData SetEnergy(float energy)
		{
			return new EnergyData(
				energy,
				Overlap,
				MoveDistance
			);
		}

		public EnergyData SetOverlap(int area)
		{
			return new EnergyData(
				Energy,
				area,
				MoveDistance
			);
		}

		public EnergyData SetMoveDistance(int moveDistance)
		{
			return new EnergyData(
				Energy,
				Overlap,
				moveDistance
			);
		}

		public EnergyData SetIsValid(bool isValid)
		{
			throw new NotImplementedException();
		}
	}
}