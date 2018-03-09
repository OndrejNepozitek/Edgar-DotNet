namespace MapGeneration.Core.Configurations.EnergyData
{
	using Interfaces.Core;
	using Interfaces.Core.Configuration.EnergyData;

	/// <inheritdoc cref="IEnergyDataCorridors" />
	/// <summary>
	/// Basic implementation of an IEnergyDataCorridors interface.
	/// </summary>
	public struct EnergyDataCorridors : IEnergyDataCorridors, ISmartCloneable<EnergyDataCorridors>
	{
		/// <inheritdoc />
		public float Energy { get; set; }

		/// <inheritdoc />
		public int Overlap { get; set; }

		/// <inheritdoc />
		public int MoveDistance { get; set; }

		/// <inheritdoc />
		public int CorridorDistance { get; set; }

		/// <inheritdoc />
		public int NumberOfTouching { get; set; }

		/// <inheritdoc />
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