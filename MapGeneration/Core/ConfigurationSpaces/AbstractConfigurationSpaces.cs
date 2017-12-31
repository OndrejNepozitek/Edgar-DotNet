namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;
	using Utils;

	public abstract class AbstractConfigurationSpaces<TNode, TShape, TConfiguration> : IConfigurationSpaces<TNode, TShape, TConfiguration>
	{
		protected Random Random = new Random();

		public void InjectRandomGenerator(Random random)
		{
			Random = random;
		}

		public IntVector2 GetRandomIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			return GetMaximumIntersection(mainConfiguration, configurations).GetRandom(Random);
		}

		public IList<IntVector2> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			throw new NotImplementedException();
		}

		protected abstract IList<Tuple<Configuration, ConfigurationSpace>> GetConfigurationSpaces(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		protected abstract ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configurations);

		public abstract TShape GetRandomShape(TNode node);

		public abstract bool CanPerturbShape(TNode node);

		public abstract IReadOnlyCollection<TShape> GetAllShapes(TNode node);

		public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
		{
			throw new NotImplementedException();
		}
	}
}