namespace MapGeneration.Core.LayoutOperations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ConfigurationSpaces;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.LayoutOperations;
	using Interfaces.Core.Layouts;
	using Interfaces.Utils;
	using Utils;

	public abstract class AbstractLayoutOperations<TLayout, TNode, TConfiguration, TShapeContainer> : ILayoutOperations<TLayout, TNode>, IRandomInjectable
		where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
		where TConfiguration : IMutableConfiguration<TShapeContainer>, ISmartCloneable<TConfiguration>
	{
		protected readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> ConfigurationSpaces;
		protected Random Random;
		protected float ShapePerturbChance = 0.4f;
		protected float DifferenceFromAverageScale = 0.4f;
		protected int AverageSize;

		protected AbstractLayoutOperations(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces, int averageSize)
		{
			ConfigurationSpaces = configurationSpaces;
			AverageSize = averageSize;
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

		public virtual void PerturbLayout(TLayout layout, IList<TNode> nodeOptions, bool updateLayout)
		{
			if (Random.NextDouble() <= ShapePerturbChance)
			{
				PerturbShape(layout, nodeOptions, updateLayout);
			}
			else
			{
				PerturbPosition(layout, nodeOptions, updateLayout);
			}
		}

		public virtual void AddChain(TLayout layout, IList<TNode> chain, bool updateLayout)
		{
			foreach (var node in chain)
			{
				AddNodeGreedily(layout, node);
			}

			if (updateLayout)
			{
				UpdateLayout(layout);
			}
		}

		public virtual bool AreDifferentEnough(TLayout layout1, TLayout layout2, IList<TNode> chain = null)
		{
			// TODO: make better
			var diff = 0d;

			var nodes = chain ?? layout1.Graph.Vertices;
			foreach (var node in nodes)
			{
				if (layout1.GetConfiguration(node, out var c1) && layout2.GetConfiguration(node, out var c2))
				{
					diff += (float)(Math.Pow(
						                5 * IntVector2.ManhattanDistance(c1.Shape.BoundingRectangle.Center + c1.Position,
							                c2.Shape.BoundingRectangle.Center + c2.Position) / (float)AverageSize, 2) * (ReferenceEquals(c1.Shape, c2.Shape) ? 1 : 4));
				}
			}

			diff = diff / (nodes.Count());

			return DifferenceFromAverageScale * diff >= 1;
		}

		public abstract bool IsLayoutValid(TLayout layout);

		public abstract bool IsLayoutValid(TLayout layout, IList<TNode> nodeOptions);

		public abstract float GetEnergy(TLayout layout);

		public abstract void UpdateLayout(TLayout layout);

		public abstract void AddNodeGreedily(TLayout layout, TNode node);

		protected abstract void UpdateLayout(TLayout layout, TNode node, TConfiguration configuration);
	}
}