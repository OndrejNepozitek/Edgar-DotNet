namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

	public interface IConfigurationSpaces<in TNode, out TShape, TConfiguration> : IRandomInjectable
	{
		IntVector2 GetRandomIntersection(IList<TConfiguration> configurations, TConfiguration mainConfiguration);

		IList<IntVector2> GetMaximumIntersection(IList<TConfiguration> configurations, TConfiguration mainConfiguration);

		TShape GetRandomShape();

		TShape GetRandomShape(TNode node);

		bool CanPerturbShape(TNode node);

		IReadOnlyCollection<TShape> GetAllShapes(TNode node);

		bool HaveValidPosition(TNode node1, TConfiguration configuration1, TNode node2, TConfiguration configuration2);
	}
}