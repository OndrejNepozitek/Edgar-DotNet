namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;
	using Utils;

	public class ConfigurationSpaces<TNode, TShape, TConfiguration> : IConfigurationSpaces<TNode, TShape, TConfiguration>
		where TConfiguration : IConfiguration<TConfiguration, TShape>
	{
		private Random random = new Random();

		public void InjectRandomGenerator(Random random)
		{
			this.random = random;
		}

		public IntVector2 GetRandomIntersection(IList<TConfiguration> configurations, TConfiguration mainConfiguration)
		{
			return GetMaximumIntersection(configurations, mainConfiguration).GetRandom(random);
		}

		public IList<IntVector2> GetMaximumIntersection(IList<TConfiguration> configurations, TConfiguration mainConfiguration)
		{
			throw new NotImplementedException();
		}

		public TShape GetRandomShape()
		{
			throw new NotImplementedException();
		}

		public TShape GetRandomShape(TNode node)
		{
			throw new NotImplementedException();
		}

		public bool CanPerturbShape(TNode node)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyCollection<TShape> GetAllShapes(TNode node)
		{
			throw new NotImplementedException();
		}

		public bool HaveValidPosition(TNode node1, TConfiguration configuration1, TNode node2, TConfiguration configuration2)
		{
			throw new NotImplementedException();
		}
	}
}
