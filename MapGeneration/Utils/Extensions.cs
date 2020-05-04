using MapGeneration.Core.ConfigurationSpaces.Interfaces;

namespace MapGeneration.Utils
{
	using System.Linq;
	using Core;
	using Core.MapDescriptions;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;

    public static class Extensions
	{
		public static int GetAverageSize<TNode, TConfiguration, TConfigurationSpace>(this ILegacyConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, TConfigurationSpace> configurationSpaces) 
		{
			return (int)configurationSpaces.GetAllShapes().Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);
		}
    }
}