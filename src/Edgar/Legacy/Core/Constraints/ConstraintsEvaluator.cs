using System.Collections.Generic;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.Configurations.Interfaces.EnergyData;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.Constraints
{
	/// <summary>
	/// Computes energy of a layout based on given constraints.
	/// </summary>
    public class ConstraintsEvaluator<TLayout, TNode, TConfiguration, TShapeContainer, TEnergyData>
        where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
        where TConfiguration : IEnergyConfiguration<TShapeContainer, TNode, TEnergyData>, ISmartCloneable<TConfiguration>, new()
        where TEnergyData : IEnergyData, new()
    {
        private readonly List<INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>> constraints;

        public ConstraintsEvaluator(List<INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>> constraints)
        {
            this.constraints = constraints;
        }

        /// <summary>
		/// Run all constraints to compute energy data for a given node and its configuration.
		/// </summary>
		/// <param name="layout"></param>
        /// <param name="configuration"></param>
		/// <returns></returns>
		public TEnergyData ComputeNodeEnergy(TLayout layout, TConfiguration configuration)
		{
			var energyData = new TEnergyData();
			var valid = true;

			foreach (var constraint in constraints)
			{
				if (!constraint.ComputeEnergyData(layout, configuration.Node, configuration, ref energyData))
				{
					valid = false;
				}
			}

			energyData.IsValid = valid;
			return energyData;
		}

		/// <summary>
		/// Computes updated energy data of a given node by computing all constraints.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="perturbedNode"></param>
		/// <param name="oldConfiguration"></param>
		/// <param name="newConfiguration"></param>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public TEnergyData UpdateNodeEnergy(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration)
		{
			var energyData = new TEnergyData();
			var valid = true;

			foreach (var constraint in constraints)
			{
				if (!constraint.UpdateEnergyData(layout, perturbedNode, oldConfiguration, newConfiguration, node, configuration, ref energyData))
				{
					valid = false;
				}
			}

			energyData.IsValid = valid;
			return energyData;
		}

		/// <summary>
		/// Computes updated energy data of the node that was perturbed.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="oldLayout"></param>
		/// <param name="newLayout"></param>
		/// <returns></returns>
		public TEnergyData UpdateNodeEnergy(TNode node, TLayout oldLayout, TLayout newLayout)
		{
			var energyData = new TEnergyData();
			var valid = true;

			foreach (var constraint in constraints)
			{
				if (!constraint.UpdateEnergyData(oldLayout, newLayout, node, ref energyData))
				{
					valid = false;
				}
			}

			energyData.IsValid = valid;
			return energyData;
		}
    }
}