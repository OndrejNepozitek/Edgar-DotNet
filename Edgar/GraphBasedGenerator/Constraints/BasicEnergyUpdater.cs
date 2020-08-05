using System;
using Edgar.GraphBasedGenerator.Configurations;
using MapGeneration.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Constraints
{
    public class BasicEnergyUpdater<TNode, TConfiguration> : IEnergyUpdater<TNode, TConfiguration, CorridorsDataNew>
    {
        private readonly float energySigma;

        public BasicEnergyUpdater(float energySigma)
        {
            this.energySigma = energySigma;
        }

        public void UpdateEnergy(ILayout<TNode, TConfiguration> layout, TConfiguration configuration, ref CorridorsDataNew energyData)
        {
            energyData.Energy = 0;
            energyData.Energy += ComputeEnergy(energyData.BasicConstraintData.Overlap, energyData.BasicConstraintData.MoveDistance);
            energyData.Energy += ComputeEnergy(0, energyData.CorridorConstraintData.CorridorDistance);
            energyData.Energy += energyData.MinimumDistanceConstraintData.WrongDistanceCount;
        }

        private float ComputeEnergy(int overlap, int distance)
        {
            return (float)(Math.Pow(Math.E, overlap / (energySigma * 625)) * Math.Pow(Math.E, distance / (energySigma * 50)) - 1);
        }
    }
}