namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;
	using Utils;

	public class LayoutOperations<TNode> : IRandomInjectable
	{
		private readonly IConfigurationSpaces<TNode> configurationSpaces;
		private Random random = new Random();

		public LayoutOperations(IConfigurationSpaces<TNode> configurationSpaces)
		{
			this.configurationSpaces = configurationSpaces;
		}

		public ILayout<TNode> PerturbShape(ILayout<TNode> layout, TNode node)
		{
			layout.GetConfiguration(node, out var configuration);

			// Return the current layout if given node cannot be shape-perturbed
			if (!configurationSpaces.CanPerturbShape(node))
				return layout;

			GridPolygon shape;
			do
			{
				shape = configurationSpaces.GetRandomShape(node);
			}
			while (ReferenceEquals(shape, configuration.Shape));

			var newConfiguration = new Grid.Configuration(configuration, polygon);

			return UpdateLayoutAfterPerturabtion(layout, node, newConfiguration);
		}

		public ILayout<TNode> PerturbShape(ILayout<TNode> layout, IList<TNode> nodeOptions)
		{
			var canBePerturbed = nodeOptions.Where(x => configurationSpaces.CanPerturbShape(x)).ToList();

			if (canBePerturbed.Count == 0)
				return layout;

			return PerturbShape(layout, canBePerturbed.GetRandom(random));
		}

		public ILayout<TNode> PerturbPosition(ILayout<TNode> layout)
		{
			throw new NotImplementedException();
		}

		public float GetEnergy(ILayout<TNode> layout)
		{
			throw new NotImplementedException();
		}

		public float GetEnergy(ILayout<TNode> layout, TNode node)
		{
			throw new NotImplementedException();
		}

		public float GetEnergy(ILayout<TNode> layout, TNode node, Configuration configuration)
		{
			throw new NotImplementedException();
		}

		public void InjectRandomGenerator(Random random)
		{
			this.random = random;
			configurationSpaces.InjectRandomGenerator(random);
		}
	}
}