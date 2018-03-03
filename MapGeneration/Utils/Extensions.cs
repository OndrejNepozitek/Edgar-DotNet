namespace MapGeneration.Utils
{
	using System.Linq;
	using Core;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.ConfigurationSpaces;

	public static class Extensions
	{
		public static int GetAverageSize<TNode, TConfiguration, TConfigurationSpace>(this IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, TConfigurationSpace> configurationSpaces) 
		{
			return (int)configurationSpaces.GetAllShapes().Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);
		} 
	}
}