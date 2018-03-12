namespace MapGeneration.Core.Configurations.EnergyData
{
	using Interfaces.Core;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Utils;

	public struct BasicEnergyData : IEnergyData, ISmartCloneable<BasicEnergyData>
	{
		public float Energy { get; set; }

		public bool IsValid { get; set; }

		public BasicEnergyData(float energy, bool isValid)
		{
			Energy = energy;
			IsValid = isValid;
		}

		public BasicEnergyData SmartClone()
		{
			return new BasicEnergyData(
				Energy,
				IsValid
			);
		}
	}
}