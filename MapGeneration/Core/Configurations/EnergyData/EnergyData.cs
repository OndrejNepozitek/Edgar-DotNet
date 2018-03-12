namespace MapGeneration.Core.Configurations.EnergyData
{
	using Interfaces.Core;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Utils;

	/// <inheritdoc cref="IEnergyData" />
	/// <summary>
	/// Basic implementation of an IEnergyData interface.
	/// </summary>
	public struct EnergyData : INodeEnergyData, ISmartCloneable<EnergyData>
	{
		/// <inheritdoc />
		public float Energy { get; set; }

		/// <inheritdoc cref="INodeEnergyData" />
		public int Overlap { get; set; }

		/// <inheritdoc cref="INodeEnergyData" />
		public int MoveDistance { get; set; }

		/// <inheritdoc />
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