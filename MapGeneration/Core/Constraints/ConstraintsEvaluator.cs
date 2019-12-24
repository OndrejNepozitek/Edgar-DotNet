using System.Collections.Generic;
using MapGeneration.Interfaces.Core.Configuration;
using MapGeneration.Interfaces.Core.Configuration.EnergyData;
using MapGeneration.Interfaces.Core.Constraints;
using MapGeneration.Interfaces.Core.Layouts;
using MapGeneration.Interfaces.Utils;

namespace MapGeneration.Core.Constraints
{
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

        public void UpdateLayout(TLayout layout, TNode perturbedNode, TConfiguration configuration)
		{
			// Prepare new layout with temporary configuration to compute energies
			var graph = layout.Graph;
			var oldLayout = layout.SmartClone(); // TODO: is the clone needed?
			oldLayout.GetConfiguration(perturbedNode, out var oldConfiguration);

			// Update validity vectors and energies of vertices
			foreach (var vertex in graph.Vertices)
			{
				if (vertex.Equals(perturbedNode))
					continue;

				if (!layout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				var vertexEnergyData = NodeRunAllUpdate(layout, perturbedNode, oldConfiguration, configuration, vertex, nodeConfiguration);

				nodeConfiguration.EnergyData = vertexEnergyData;
				layout.SetConfiguration(vertex, nodeConfiguration);
			}

			// Update energy and validity vector of the perturbed node
			var newEnergyData = NodeRunAllUpdate(perturbedNode, oldLayout, layout);
			configuration.EnergyData = newEnergyData;
			layout.SetConfiguration(perturbedNode, configuration);
		}

		/// <summary>
		/// Computes energy data of a give node with a given configuration.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public TEnergyData NodeComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration)
		{
			return NodeRunAllCompute(layout, node, configuration);
		}

		/// <summary>
		/// Run all constraints to compute energy data for a given node and configuration.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public TEnergyData NodeRunAllCompute(TLayout layout, TNode node, TConfiguration configuration)
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
			return energyData;
		}

		/// <summary>
		/// Computes updated energy data of a given node by running all constraints.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="perturbedNode"></param>
		/// <param name="oldConfiguration"></param>
		/// <param name="newConfiguration"></param>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public TEnergyData NodeRunAllUpdate(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration)
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
		public TEnergyData NodeRunAllUpdate(TNode node, TLayout oldLayout, TLayout newLayout)
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