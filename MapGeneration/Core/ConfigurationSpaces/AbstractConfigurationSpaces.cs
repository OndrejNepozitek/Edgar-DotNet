namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;
	using Utils;

	public abstract class AbstractConfigurationSpaces<TNode, TShapeContainer, TConfiguration> : IConfigurationSpaces<TNode, TShapeContainer, TConfiguration>
		where TConfiguration : IConfiguration<TConfiguration, TShapeContainer>
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
			var intersection = GetMaximumIntersection(mainConfiguration, configurations);

			// TODO: maybe make it in a way that not all points are generated
			return intersection.GetWeightedRandom(x => x.Length + 1, Random).GetPoints().GetRandom(Random);
		}

		public IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
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
						return intersection;
					}
				}
			}

			throw new InvalidOperationException("There should always be at least one point in the intersection");
		}

		protected abstract IList<Tuple<TConfiguration, ConfigurationSpace>> GetConfigurationSpaces(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		protected abstract ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configurations);

		public abstract TShapeContainer GetRandomShape(TNode node);

		public abstract bool CanPerturbShape(TNode node);

		public abstract IReadOnlyCollection<TShapeContainer> GetShapesForNode(TNode node);

		public abstract IEnumerable<TShapeContainer> GetAllShapes();

		public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
		{
			var space = GetConfigurationSpace(configuration2, configuration1);
			var lines1 = new List<OrthogonalLine>() {new OrthogonalLine(configuration1.Position, configuration1.Position)};

			return LineIntersection.DoIntersect(lines1, space.Lines.Select(x => x + configuration2.Position).ToList());
		}
	}
}