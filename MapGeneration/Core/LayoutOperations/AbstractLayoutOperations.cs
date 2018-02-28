namespace MapGeneration.Core.LayoutOperations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ConfigurationSpaces;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.ConfigurationSpaces;
	using Utils;

	public abstract class AbstractLayoutOperations<TLayout, TNode, TConfiguration, TShapeContainer> : ILayoutOperations<TLayout, TNode>
		where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
		where TConfiguration : IMutableConfiguration<TShapeContainer>, ISmartCloneable<TConfiguration>
	{
		protected readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> ConfigurationSpaces;
		protected Random Random = new Random();

		protected AbstractLayoutOperations(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces)
		{
			ConfigurationSpaces = configurationSpaces;
		}

		public virtual void InjectRandomGenerator(Random random)
		{
			Random = random;
		}

		public virtual void PerturbShape(TLayout layout, TNode node, bool updateLayout)
		{
			layout.GetConfiguration(node, out var configuration);

			// Return the current layout if a given node cannot be shape-perturbed
			if (!ConfigurationSpaces.CanPerturbShape(node))
				return;

			TShapeContainer shape;
			do
			{
				shape = ConfigurationSpaces.GetRandomShape(node);
			}
			while (ReferenceEquals(shape, configuration.Shape));

			var newConfiguration = configuration.SmartClone();
			newConfiguration.ShapeContainer = shape;

			if (updateLayout)
			{
				UpdateLayout(layout, node, newConfiguration);
				return;
			}

			layout.SetConfiguration(node, newConfiguration);
		}

		public virtual void PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateLayout)
		{
			var canBePerturbed = nodeOptions.Where(x => ConfigurationSpaces.CanPerturbShape(x)).ToList();

			if (canBePerturbed.Count == 0)
				return;

			PerturbShape(layout, canBePerturbed.GetRandom(Random), updateLayout);
		}

		public virtual void PerturbPosition(TLayout layout, TNode node, bool updateLayout)
		{
			var configurations = new List<TConfiguration>();

			foreach (var neighbour in layout.Graph.GetNeighbours(node))
			{
				if (layout.GetConfiguration(neighbour, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			if (!layout.GetConfiguration(node, out var mainConfiguration))
				throw new InvalidOperationException();

			var newPosition = ConfigurationSpaces.GetRandomIntersectionPoint(mainConfiguration, configurations);
			var newConfiguration = mainConfiguration.SmartClone();
			newConfiguration.Position = newPosition;

			if (updateLayout)
			{
				UpdateLayout(layout, node, newConfiguration);
				return;
			}

			layout.SetConfiguration(node, newConfiguration);
		}

		public virtual void PerturbPosition(TLayout layout, IList<TNode> nodeOptions, bool updateLayout)
		{
			// TODO: check what would happen if only invalid nodes could be perturbed
			var canBePerturbed = nodeOptions.ToList();

			if (canBePerturbed.Count == 0)
				return;

			PerturbPosition(layout, canBePerturbed.GetRandom(Random), updateLayout);
		}

		public abstract bool IsLayoutValid(TLayout layout);

		public abstract float GetEnergy(TLayout layout);

		public abstract void UpdateLayout(TLayout layout);

		public abstract void AddNodeGreedily(TLayout layout, TNode node);

		protected abstract void UpdateLayout(TLayout layout, TNode node, TConfiguration configuration);
	}
}