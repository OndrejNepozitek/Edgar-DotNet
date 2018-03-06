namespace MapGeneration.Core.Configurations.EnergyData
{
	using Interfaces.Core;
	using Interfaces.Core.Configuration.EnergyData;

	public struct EnergyDataCorridors : IEnergyDataCorridors, ISmartCloneable<EnergyDataCorridors>
	{
		public float Energy { get; set; }

		public int Overlap { get; set; }

		public int MoveDistance { get; set; }

		public int CorridorDistance { get; set; }

		public int NumberOfTouching { get; set; }

		public bool IsValid { get; set; }

		public EnergyDataCorridors(float energy, int overlap, int moveDistance, bool isValid, int corridorDistance, int numberOfTouching)
		{
			Energy = energy;
			Overlap = overlap;
			MoveDistance = moveDistance;
			CorridorDistance = corridorDistance;
			IsValid = isValid;
			NumberOfTouching = numberOfTouching;
		}

		public EnergyDataCorridors SmartClone()
		{
			return new EnergyDataCorridors(
				Energy,
				Overlap,
				MoveDistance,
				IsValid,
				CorridorDistance,
				NumberOfTouching
			);
		}
	}
}