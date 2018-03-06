namespace MapGeneration.Core.Configurations.EnergyData
{
	using Interfaces.Core;
	using Interfaces.Core.Configuration.EnergyData;

	public struct EnergyData : IEnergyData, ISmartCloneable<EnergyData>
	{
		public float Energy { get; set; }

		public int Overlap { get; set; }

		// We need an int here because there must be no rounding of the value
		public int MoveDistance { get; set; }

		public bool IsValid { get; set; }

		public EnergyData(float energy, int area, int moveDistance, bool isValid)
		{
			Energy = energy;
			Overlap = area;
			MoveDistance = moveDistance;
			IsValid = isValid;
		}

		public EnergyData SmartClone()
		{
			return new EnergyData(
				Energy,
				Overlap,
				MoveDistance,
				IsValid
			);
		}
	}
}