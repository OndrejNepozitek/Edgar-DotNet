using Edgar.Legacy.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Common.Constraints.Interfaces
{
    public interface IRoomConstraint<TRoom, TConfiguration, TEnergyData>
    {
        void UpdateConstraintData(ILayout<TRoom, TConfiguration> layout, TRoom changedNode,
            TConfiguration oldConfiguration, TConfiguration newConfiguration);

        void ComputeConstraintData(ILayout<TRoom, TConfiguration> layout);

        bool ComputeNodeConstraintData(ILayout<TRoom, TConfiguration> layout, TRoom room, TConfiguration configuration,
            ref TEnergyData energyData);
    }
}