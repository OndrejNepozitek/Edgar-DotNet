namespace MapGeneration.Utils
{
	using System.Linq;
	using Core;
	using Core.MapDescriptions;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.ConfigurationSpaces;

	public static class Extensions
	{
		public static int GetAverageSize<TNode, TConfiguration, TConfigurationSpace>(this IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, TConfigurationSpace> configurationSpaces) 
		{
			return (int)configurationSpaces.GetAllShapes().Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);
		}

		public static MapDescriptionOld<TNode> SetupWithGraph<TNode>(this MapDescriptionOld<TNode> mapDescriptionOld, IGraph<TNode> graph)
		{
			foreach (var vertex in graph.Vertices)
			{
				mapDescriptionOld.AddRoom(vertex);
			}

			foreach (var edge in graph.Edges)
			{
				mapDescriptionOld.AddPassage(edge.From, edge.To);
			}

			return mapDescriptionOld;
		}
    }
}