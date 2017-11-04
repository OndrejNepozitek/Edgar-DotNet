namespace MapGeneration.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using Layouts;

	public interface IConfigurationSpaces<TPolygon, in TNode> where TNode : IComparable<TNode>
	{
		ILayout<TPolygon> PerturbLayout(ILayout<TPolygon> layout);

		ILayout<TPolygon> AddChain(ILayout<TPolygon> layout, IEnumerable<TNode> chain);

		ILayout<TPolygon> GetInitialLayout(IEnumerable<TNode> chain);
	}
}
