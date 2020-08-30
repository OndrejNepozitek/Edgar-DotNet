using Edgar.GraphBasedGenerator.Common.Constraints.BasicConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.CorridorConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.MinimumDistanceConstraint;
using Edgar.Legacy.Core.Configurations.Interfaces.EnergyData;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Common.Configurations
{
	/// <inheritdoc cref="ICorridorsData" />
	/// <summary>
	/// Basic implementation of an IEnergyDataCorridors interface.
	/// </summary>
	public struct EnergyData : IEnergyData, IBasicConstraintData, ICorridorConstraintData, IMinimumDistanceConstraintData, ISmartCloneable<EnergyData>
	{
        /// <inheritdoc />
		public float Energy { get; set; }
		
		/// <inheritdoc />
		public bool IsValid { get; set; }

        public BasicConstraintData BasicConstraintData { get; set; }

        public CorridorConstraintData CorridorConstraintData { get; set; }

        public MinimumDistanceConstraintData MinimumDistanceConstraintData { get; set; }

		public EnergyData(float energy, bool isValid, BasicConstraintData basicConstraintData, CorridorConstraintData corridorConstraintData, MinimumDistanceConstraintData minimumDistanceConstraintData)
		{
			Energy = energy;
            IsValid = isValid;
            BasicConstraintData = basicConstraintData;
            CorridorConstraintData = corridorConstraintData;
            MinimumDistanceConstraintData = minimumDistanceConstraintData;
        }

		public EnergyData SmartClone()
		{
			return new EnergyData(
				Energy,
                IsValid,
                BasicConstraintData,
				CorridorConstraintData,
				MinimumDistanceConstraintData
			);
		}
    }
}