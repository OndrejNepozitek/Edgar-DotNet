using System.Linq;
using Edgar.Legacy.Core.ConfigurationSpaces.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Utils
{
    public static class Extensions
	{
		public static int GetAverageSize<TNode, TConfiguration, TConfigurationSpace>(this IConfigurationSpaces<TNode, IntAlias<PolygonGrid2D>, TConfiguration, TConfigurationSpace> configurationSpaces) 
		{
			return (int)configurationSpaces.GetAllShapes().Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);
		}
    }
}