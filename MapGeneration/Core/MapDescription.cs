namespace MapGeneration.Core
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;

	public class MapDescription<TNode> : IMapDescription<TNode>
	{
		public IReadOnlyCollection<IRoomDescription> GetRooms()
		{
			throw new System.NotImplementedException();
		}

		public FastGraph<TNode> GetGraph()
		{
			throw new System.NotImplementedException();
		}
	}
}