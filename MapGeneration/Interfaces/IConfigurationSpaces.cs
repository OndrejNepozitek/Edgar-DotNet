namespace MapGeneration.Interfaces
{
	using System;
	using System.Collections.Generic;

	public interface IConfigurationSpaces<TPolygon, TPosition, in TNode, TConfifuration>
		where TConfifuration : IConfiguration<TPolygon, TPosition>
	{
		TPosition GetRandomIntersection(List<TConfifuration> configurations, TConfifuration mainConfiguration);

		List<TPosition> GetMaximumIntersection(List<TConfifuration> configurations, TConfifuration mainConfiguration);

		TPolygon GetRandomShape();

		TPolygon GetRandomShape(TNode node);

		bool CanPerturbShape(TNode node);

		ICollection<TPolygon> GetAllShapes();

		void InjectRandomGenerator(Random random);

		bool HaveValidPosition(TConfifuration configuration1, TConfifuration configuration2);
	}
}
