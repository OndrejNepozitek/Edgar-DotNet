namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;

	public interface ILayoutOperations<TLayout, TNode> : IRandomInjectable
	{
		TLayout PerturbShape(TLayout layout, TNode node, bool updateLayout);

		TLayout PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		TLayout PerturbPosition(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		TLayout PerturbPosition(TLayout layout, TNode node, bool updateLayout);

		bool IsLayoutValid(TLayout layout);

		float GetEnergy(TLayout layout);

		void UpdateLayout(TLayout layout);

		void AddNodeGreedily(TLayout layout, TNode node);
	}
}