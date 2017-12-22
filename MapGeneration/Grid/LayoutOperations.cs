namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using Interfaces;

	public class LayoutOperations<TNode, TPolygon, TPosition, TDoor>
	{
		private IConfigurationSpaces<TPolygon, TPosition, TNode> configurationSpaces;

		public ILayout<TNode, TPolygon, TPosition, TDoor> PerturbShape(ILayout<TNode, TPolygon, TPosition, TDoor> layout, TNode node)
		{
			layout.GetConfiguration(node, out var configuration);

			// Return the current layout if given node cannot be shape-perturbed
			if (!configurationSpaces.CanPerturbShape(node))
				return layout;

			TPolygon shape;
			do
			{
				shape = configurationSpaces.GetRandomShape(node);
			}
			while (ReferenceEquals(shape, configuration.Shape));

			var newConfiguration = new Configuration(configuration, polygon);

			return UpdateLayoutAfterPerturabtion(layout, node, newConfiguration);
		}

		public ILayout<TNode, TPolygon, TPosition, TDoor-> PerturbShape(ILayout<TNode, TPolygon, TPosition, TDoor> layout)
		{
			throw new NotImplementedException();
		}

		public ILayout<TNode, TPolygon, TPosition, TDoor> PerturbShape(ILayout<TNode, TPolygon, TPosition, TDoor> layout, List<TNode> nodeOptions)
		{
			throw new NotImplementedException();
		}

		public ILayout<TNode, TPolygon, TPosition, TDoor> PerturbPosition(ILayout<TNode, TPolygon, TPosition, TDoor> layout)
		{
			throw new NotImplementedException();
		}
	}
}