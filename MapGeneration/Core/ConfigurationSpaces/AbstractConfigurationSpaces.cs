namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.ConfigurationSpaces;
	using Utils;

	public abstract class AbstractConfigurationSpaces<TNode, TShapeContainer, TConfiguration> : IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace>, IRandomInjectable
		where TConfiguration : IConfiguration<TShapeContainer>
	{
		protected Random Random = new Random();
		protected ILineIntersection<OrthogonalLine> LineIntersection;

		protected AbstractConfigurationSpaces(ILineIntersection<OrthogonalLine> lineIntersection)
		{
			LineIntersection = lineIntersection;
		}

		public void InjectRandomGenerator(Random random)
		{
			Random = random;
		}

		public IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			return GetRandomIntersectionPoint(mainConfiguration, configurations, out var configurationsSatisfied);
		}

		public IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied)
		{
			var intersection = GetMaximumIntersection(mainConfiguration, configurations, out configurationsSatisfied);

			var line = intersection.GetWeightedRandom(x => x.Length + 1, Random);
			return line.GetNthPoint(Random.Next(line.Length + 1));
			//return GetRandomPoint(intersection.GetWeightedRandom(x => x.Length > 0 ? x.Length / 25 + 1 : 1, Random));
		}

		private IntVector2 GetRandomPoint(OrthogonalLine line)
		{
			var length = line.Length + 1;
			var wantedChoices = length;

			if (length <= wantedChoices)
			{
				return line.GetNthPoint(Random.Next(length));
			}

			var partsSize = length / wantedChoices;
			var partsCount = length / partsSize;
			var randomPart = Random.Next(partsCount + 1);

			if (randomPart != partsCount)
			{
				return line.GetNthPoint(randomPart * partsSize);
			}

			return line.GetNthPoint(length - 1);
		}

		public IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			return GetMaximumIntersection(mainConfiguration, configurations, out var configurationsSatisfied);
		}

		public IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied)
		{
			var spaces = GetConfigurationSpaces(mainConfiguration, configurations);

			for (var i = configurations.Count; i > 0; i--)
			{
				foreach (var indices in configurations.GetCombinations(i))
				{
					List<OrthogonalLine> intersection = null;
					// TODO: Would it be better if GetIntersections was lazy?

					foreach (var index in indices)
					{
						var linesToIntersect = spaces[index].Item2.Lines.Select(x => x + spaces[index].Item1.Position).ToList();
						intersection = intersection != null
							? LineIntersection.GetIntersections(linesToIntersect, intersection)
							: linesToIntersect;

						if (intersection.Count == 0)
						{
							break;
						}
					}

					if (intersection != null && intersection.Count != 0)
					{
						configurationsSatisfied = indices.Length;
						return intersection;
					}
				}
			}

			throw new InvalidOperationException("There should always be at least one point in the intersection");
		}

		protected abstract IList<Tuple<TConfiguration, ConfigurationSpace>> GetConfigurationSpaces(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		protected abstract ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configurations);

		public abstract ConfigurationSpace GetConfigurationSpace(TShapeContainer shape1, TShapeContainer shape2);

		public abstract TShapeContainer GetRandomShape(TNode node);

		public abstract bool CanPerturbShape(TNode node);

		public abstract IReadOnlyCollection<TShapeContainer> GetShapesForNode(TNode node);

		public abstract IEnumerable<TShapeContainer> GetAllShapes();

		public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
		{
			var space = GetConfigurationSpace(configuration1, configuration2);
			var lines1 = new List<OrthogonalLine>() {new OrthogonalLine(configuration1.Position, configuration1.Position)};

			return LineIntersection.DoIntersect(space.Lines.Select(x => FastAddition(x, configuration2.Position)), lines1);
		}

		private OrthogonalLine FastAddition(OrthogonalLine line, IntVector2 position) 
		{
			return new OrthogonalLine(line.From + position, line.To + position);
		}
	}
}