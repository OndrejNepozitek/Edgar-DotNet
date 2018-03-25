namespace MapGeneration.Core.LayoutConverters.CorridorNodesCreators
{
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core.LayoutConverters;
	using Interfaces.Core.MapDescriptions;

	/// <inheritdoc />
	public class StringCorridorNodesCreator : ICorridorNodesCreator<string>
	{
		private readonly string prefix;

		public StringCorridorNodesCreator(string prefix = "corridor_")
		{
			this.prefix = prefix;
		}

		/// <inheritdoc />
		/// <remarks>
		/// Adds number to a given prefix until all corridors have a name.
		/// </remarks>
		public void AddCorridorsToMapping(ICorridorMapDescription<int> mapDescription, TwoWayDictionary<string, int> mapping)
		{
			var graph = mapDescription.GetGraph();
			var corridors = graph.Vertices.Where(mapDescription.IsCorridorRoom).ToList();

			var counter = 0;

			foreach (var corridor in corridors)
			{
				while (true)
				{
					var name = prefix + counter;

					if (!mapping.ContainsKey(name))
					{
						mapping.Add(name, corridor);
						break;
					}

					counter++;
				}
			}
		}
	}
}