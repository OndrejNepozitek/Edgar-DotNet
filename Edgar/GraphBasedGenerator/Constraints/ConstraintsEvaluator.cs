using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Constraints.Interfaces;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Constraints
{
    public class ConstraintsEvaluator<TRoom, TConfiguration, TEnergyData>
        where TEnergyData: IEnergyData, new() // TODO: revise later
    {
        private readonly List<IRoomConstraint<TRoom, TConfiguration, TEnergyData>> constraints;

        public ConstraintsEvaluator(List<IRoomConstraint<TRoom, TConfiguration, TEnergyData>> constraints)
        {
            this.constraints = constraints;
        }

        public TEnergyData ComputeNodeEnergyData(ILayout<TRoom, TConfiguration> layout, TRoom room, TConfiguration configuration)
        {
            var energyData = new TEnergyData();
            var valid = true;

            foreach (var constraint in constraints)
            {
                if (!constraint.ComputeNodeConstraintData(layout, room, configuration, ref energyData))
                {
                    valid = false;
                }
            }

            energyData.IsValid = valid;
            return energyData;
        }
    }
}