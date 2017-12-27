namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;
	using Utils;

	public class LayoutOperations<TNode, TLayout> : IRandomInjectable
		where TLayout : ILayout<TNode>
	{
		private readonly IConfigurationSpaces<TNode> configurationSpaces;
		private Random random = new Random();

		public LayoutOperations(IConfigurationSpaces<TNode> configurationSpaces)
		{
			this.configurationSpaces = configurationSpaces;
		}

		public TLayout PerturbShape(TLayout layout, TNode node)
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

		public TLayout PerturbShape(TLayout layout, IList<TNode> nodeOptions)
		{
			var canBePerturbed = nodeOptions.Where(x => configurationSpaces.CanPerturbShape(x)).ToList();

			if (canBePerturbed.Count == 0)
				return layout;

			return PerturbShape(layout, canBePerturbed.GetRandom(random));
		}

		public TLayout PerturbPosition(TLayout layout)
		{
			throw new NotImplementedException();
		}

		public float GetEnergy(TLayout layout)
		{
			throw new NotImplementedException();
		}

		public float GetEnergy(TLayout layout, TNode node)
		{
			throw new NotImplementedException();
		}

		public float GetEnergy(TLayout layout, TNode node, Configuration configuration)
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