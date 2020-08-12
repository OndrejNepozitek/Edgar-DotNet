using System.Collections.Generic;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.Legacy.Core.Layouts.Interfaces
{
    /// <summary>
	/// Represents a layout that holds configurations of nodes.
	/// </summary>
	/// <remarks>
	/// No nodes have configuration by default. Their configurations must be first
	/// added with a SetConfiguration method.
	/// </remarks>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
	public interface ILayout<TNode, TConfiguration> 
	{
		/// <summary>
		/// Underlying graph of nodes connections.
		/// </summary>
		IGraph<TNode> Graph { get; }

		/// <summary>
		/// Gets the configuration of a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns>True if a given node has already a configuration.</returns>
		bool GetConfiguration(TNode node, out TConfiguration configuration);

		/// <summary>
		/// Sets a configurations of a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		void SetConfiguration(TNode node, TConfiguration configuration);

		/// <summary>
		/// Removes configuration of a given node.
		/// </summary>
		/// <param name="node"></param>
		void RemoveConfiguration(TNode node);

		/// <summary>
		/// Gets all configurations.
		/// </summary>
		/// <returns></returns>
		IEnumerable<TConfiguration> GetAllConfigurations();
	}
}