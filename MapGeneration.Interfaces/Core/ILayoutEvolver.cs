namespace MapGeneration.Interfaces.Core
{
	using System;
	using System.Collections.Generic;

	public interface ILayoutEvolver<TLayout, TNode>
	{
		event Action<TLayout> OnPerturbed;

		event Action<TLayout> OnValid;

		IEnumerable<TLayout> Evolve(TLayout layout, IList<TNode> chain, int count);
	}
}