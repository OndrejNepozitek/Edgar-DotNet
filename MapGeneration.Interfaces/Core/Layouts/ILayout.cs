namespace MapGeneration.Interfaces.Core.Layouts
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ILayout<TNode, TConfiguration> 
	{
		IGraph<TNode> Graph { get; }

		bool GetConfiguration(TNode node, out TConfiguration configuration);

		void SetConfiguration(TNode node, TConfiguration configuration);

		void RemoveConfiguration(TNode node);

		IEnumerable<TConfiguration> GetAllConfigurations();
	}
}