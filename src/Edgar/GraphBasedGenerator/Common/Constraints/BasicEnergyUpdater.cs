using System;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.Legacy.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Common.Constraints
{
    public class BasicEnergyUpdater<TNode, TConfiguration> : IEnergyUpdater<TNode, TConfiguration, EnergyData>
    {
        private readonly float energySigma;

        public BasicEnergyUpdater(float energySigma)
        {
            this.energySigma = energySigma;
        }

        public void UpdateEnergy(ILayout<TNode, TConfiguration> layout, TConfiguration configuration,
            ref EnergyData energyData)
        {
            energyData.Energy = 0;
            energyData.Energy += ComputeEnergy(energyData.BasicConstraintData.Overlap,
                energyData.BasicConstraintData.MoveDistance);
            energyData.Energy += ComputeEnergy(0, energyData.CorridorConstraintData.CorridorDistance);
            energyData.Energy += energyData.MinimumDistanceConstraintData.WrongDistanceCount;
        }

        private float ComputeEnergy(int overlap, int distance)
        {
            return (float) (Math.Pow(Math.E, overlap / (energySigma * 625)) *
                Math.Pow(Math.E, distance / (energySigma * 50)) - 1);
        }
    }
}