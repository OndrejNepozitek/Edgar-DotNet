namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

	public interface IConfigurationSpaces<in TNode, out TShape, TConfiguration> : IRandomInjectable
	{
		IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		TShape GetRandomShape(TNode node);

		bool CanPerturbShape(TNode node);

		IReadOnlyCollection<TShape> GetAllShapes(TNode node);

		bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);
	}
}