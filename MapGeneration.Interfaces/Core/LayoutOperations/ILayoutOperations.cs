namespace MapGeneration.Interfaces.Core.LayoutOperations
{
	using System.Collections.Generic;

	public interface ILayoutOperations<in TLayout, TNode>
	{
		void PerturbShape(TLayout layout, TNode node, bool updateLayout);

		void PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		void PerturbPosition(TLayout layout, TNode node, bool updateLayout);

		void PerturbPosition(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		void PerturbLayout(TLayout layout, IList<TNode> nodeOptions, bool updateLayout);

		bool IsLayoutValid(TLayout layout);

		bool IsLayoutValid(TLayout layout, IList<TNode> nodeOptions); // TODO: remove

		float GetEnergy(TLayout layout);

		void UpdateLayout(TLayout layout);

		void AddNodeGreedily(TLayout layout, TNode node);

		void AddChain(TLayout layout, IList<TNode> chain, bool updateLayout);

		bool AreDifferentEnough(TLayout layout1, TLayout layout2, IList<TNode> chain = null);
	}
}