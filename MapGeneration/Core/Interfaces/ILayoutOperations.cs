namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;

	public interface ILayoutOperations<in TLayout, TNode> : IRandomInjectable
	{
		void PerturbShape(TLayout layout, TNode node, bool updateLayout);

		void PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		void PerturbPosition(TLayout layout, TNode node, bool updateLayout);

		void PerturbPosition(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		bool IsLayoutValid(TLayout layout);

		float GetEnergy(TLayout layout);

		void UpdateLayout(TLayout layout);

		void AddNodeGreedily(TLayout layout, TNode node);
	}
}