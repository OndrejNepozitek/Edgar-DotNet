using Edgar.GraphBasedGenerator.Constraints.BasicConstraint;
using Edgar.GraphBasedGenerator.Constraints.CorridorConstraint;
using Edgar.GraphBasedGenerator.Constraints.MinimumDistanceConstraint;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Configurations
{
	/// <inheritdoc cref="ICorridorsData" />
	/// <summary>
	/// Basic implementation of an IEnergyDataCorridors interface.
	/// </summary>
	public struct CorridorsDataNew : IEnergyData, IBasicConstraintData, ICorridorConstraintData, IMinimumDistanceConstraintData, ISmartCloneable<CorridorsDataNew>
	{
        /// <inheritdoc />
		public float Energy { get; set; }
		
		/// <inheritdoc />
		public bool IsValid { get; set; }

        public BasicConstraintData BasicConstraintData { get; set; }

        public CorridorConstraintData CorridorConstraintData { get; set; }

        public MinimumDistanceConstraintData MinimumDistanceConstraintData { get; set; }

		public CorridorsDataNew(float energy, bool isValid, BasicConstraintData basicConstraintData, CorridorConstraintData corridorConstraintData, MinimumDistanceConstraintData minimumDistanceConstraintData)
		{
			Energy = energy;
            IsValid = isValid;
            BasicConstraintData = basicConstraintData;
            CorridorConstraintData = corridorConstraintData;
            MinimumDistanceConstraintData = minimumDistanceConstraintData;
        }

		public CorridorsDataNew SmartClone()
		{
			return new CorridorsDataNew(
				Energy,
                IsValid,
                BasicConstraintData,
				CorridorConstraintData,
				MinimumDistanceConstraintData
			);
		}
    }
}