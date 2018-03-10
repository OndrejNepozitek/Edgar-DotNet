namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System;

	/// <inheritdoc />
	/// <summary>
	/// Interfaces for layout generated that can be observed for example from GUI.
	/// </summary>
	/// <typeparam name="TMapDescription"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	public interface IObservableGenerator<in TMapDescription, TNode> : ILayoutGenerator<TMapDescription, TNode>
	{
		/// <summary>
		/// Invoked whenever a layout is perturbed.
		/// </summary>
		event Action<IMapLayout<TNode>> OnPerturbed;

		/// <summary>
		/// Invoked whenever a partial valid layout is found.
		/// </summary>
		event Action<IMapLayout<TNode>> OnPartialValid;

		/// <summary>
		/// Invoked whenever a valid layout is found.
		/// </summary>
		event Action<IMapLayout<TNode>> OnValid;
	}
}