namespace MapGeneration.Core.LayoutConverters.CorridorNodesCreators
{
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core.LayoutConverters;
	using Interfaces.Core.MapDescriptions;

	/// <inheritdoc />
	public class IntCorridorNodesCreator : ICorridorNodesCreator<int>
	{
		private readonly int roundTo;

		public IntCorridorNodesCreator(int roundTo = 1000)
		{
			this.roundTo = roundTo;
		}

		/// <inheritdoc />
		/// <summary>
		/// Finds the smallest feasible multiple of a specified number and then make from it a sequence of corridors.
		/// </summary>
		public void AddCorridorsToMapping(ICorridorMapDescription<int> mapDescription, TwoWayDictionary<int, int> mapping)
		{
			var graph = mapDescription.GetGraph();
			var corridors = graph.Vertices.Where(mapDescription.IsCorridorRoom).ToList();
			var maxNonCorridorNumber = graph.Vertices.Where(x => !mapDescription.IsCorridorRoom(x)).Max();

			var parts = maxNonCorridorNumber / roundTo;
			var nextNumber = roundTo * (parts + 1);
			var counter = nextNumber;

			foreach (var corridor in corridors)
			{
				mapping.Add(counter++, corridor);
			}
		}
	}
}