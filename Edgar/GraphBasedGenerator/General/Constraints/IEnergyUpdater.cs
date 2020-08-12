using MapGeneration.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.General.Constraints
{
    public interface IEnergyUpdater<TNode, TConfiguration, TEnergyData>
    {
        void UpdateEnergy(ILayout<TNode, TConfiguration> layout, TConfiguration configuration, ref TEnergyData energyData);
    }
}