namespace MapGeneration.Core.Configurations.EnergyData
{
	using Interfaces.Core;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Utils;

	/// <inheritdoc cref="ICorridorsData" />
	/// <summary>
	/// Basic implementation of an IEnergyDataCorridors interface.
	/// </summary>
	public struct CorridorsData : ICorridorsData, ISmartCloneable<CorridorsData>
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

		public CorridorsData(float energy, int overlap, int moveDistance, bool isValid, int corridorDistance, int numberOfTouching)
		{
			Energy = energy;
			Overlap = overlap;
			MoveDistance = moveDistance;
			CorridorDistance = corridorDistance;
			IsValid = isValid;
			NumberOfTouching = numberOfTouching;
		}

		public CorridorsData SmartClone()
		{
			return new CorridorsData(
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