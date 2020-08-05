using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Configurations;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Constraints
{
	/// <summary>
	/// Computes energy of a layout based on given constraints.
	/// </summary>
    public class ConstraintsEvaluator<TNode, TConfiguration, TEnergyData>
        where TConfiguration : ISimpleEnergyConfiguration<TEnergyData>, ISmartCloneable<TConfiguration>, new()
        where TEnergyData : IEnergyData, new()
    {
        private readonly List<INodeConstraint<ILayout<TNode, TConfiguration>, TNode, TConfiguration, TEnergyData>> constraints;
        private readonly IEnergyUpdater<TNode, TConfiguration, TEnergyData> energyUpdater;

        public ConstraintsEvaluator(List<INodeConstraint<ILayout<TNode, TConfiguration>, TNode, TConfiguration, TEnergyData>> constraints, IEnergyUpdater<TNode, TConfiguration, TEnergyData> energyUpdater)
        {
            this.constraints = constraints;
            this.energyUpdater = energyUpdater;
        }

        /// <summary>
		/// Run all constraints to compute energy data for a given node and its configuration.
		/// </summary>
		/// <param name="layout"></param>
        /// <param name="configuration"></param>
		/// <returns></returns>
		public TEnergyData ComputeNodeEnergy(ILayout<TNode, TConfiguration> layout, TNode node, TConfiguration configuration)
		{
			var energyData = new TEnergyData();
			var valid = true;

			foreach (var constraint in constraints)
			{
				if (!constraint.ComputeEnergyData(layout, node, configuration, ref energyData))
				{
					valid = false;
				}
			}

			energyData.IsValid = valid;

			energyUpdater.UpdateEnergy(layout, configuration, ref energyData);

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
		public TEnergyData UpdateNodeEnergy(ILayout<TNode, TConfiguration> layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration)
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

            energyUpdater.UpdateEnergy(layout, configuration, ref energyData);

			return energyData;
		}

		/// <summary>
		/// Computes updated energy data of the node that was perturbed.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="oldLayout"></param>
		/// <param name="newLayout"></param>
		/// <returns></returns>
		public TEnergyData UpdateNodeEnergy(TNode node, ILayout<TNode, TConfiguration> oldLayout, ILayout<TNode, TConfiguration> newLayout)
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

			// TODO: weird
            newLayout.GetConfiguration(node, out var configuration);
            energyUpdater.UpdateEnergy(newLayout, configuration, ref energyData);

			return energyData;
		}
    }
}