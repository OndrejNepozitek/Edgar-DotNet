namespace MapGeneration.Interfaces
{
	using System;
	using System.Collections.Generic;

	public interface IConfigurationSpaces<TPolygon, TConfiguration, TPoint>
	{
		TPoint GetRandomIntersection(List<TConfiguration> configurations, TConfiguration mainConfiguration);

		List<TPoint> GetMaximumIntersection(List<TConfiguration> configurations, TConfiguration mainConfiguration);

		TPolygon GetRandomShape();

		ICollection<TPolygon> GetAllShapes();

		void InjectRandomGenerator(Random random);

		bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);
	}
}
