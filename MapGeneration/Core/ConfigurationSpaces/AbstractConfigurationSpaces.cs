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
	using Interfaces.Utils;
	using Utils;

	/// <inheritdoc cref="IConfigurationSpaces{TNode,TShape,TConfiguration,TConfigurationSpace}" />
	/// <summary>
	/// Abstract class for configuration spaces with common methods already implemented.
	/// </summary>
	public abstract class AbstractConfigurationSpaces<TNode, TShapeContainer, TConfiguration> : IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace>, IRandomInjectable
		where TConfiguration : IConfiguration<TShapeContainer>
	{
		protected Random Random;
		protected ILineIntersection<OrthogonalLine> LineIntersection;

		protected AbstractConfigurationSpaces(ILineIntersection<OrthogonalLine> lineIntersection)
		{
			LineIntersection = lineIntersection;
		}

		/// <inheritdoc />
		public void InjectRandomGenerator(Random random)
		{
			Random = random;
		}

		/// <inheritdoc />
		public IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			return GetRandomIntersectionPoint(mainConfiguration, configurations, out var configurationsSatisfied);
		}

		/// <inheritdoc />
		public IntVector2 GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied)
		{
			var intersection = GetMaximumIntersection(mainConfiguration, configurations, out configurationsSatisfied);

			var line = intersection.GetWeightedRandom(x => x.Length + 1, Random);
			return line.GetNthPoint(Random.Next(line.Length + 1));
		}

		/// <inheritdoc />
		public IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			return GetMaximumIntersection(mainConfiguration, configurations, out var configurationsSatisfied);
		}

		/// <inheritdoc />
		/// <remarks>
		/// Tries possible combinations of given configurations until an intersection is found.
		/// Throws when no intersecion was found.
		/// </remarks>
		public IList<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied)
		{
			var spaces = GetConfigurationSpaces(mainConfiguration, configurations);
			spaces.Shuffle(Random);

			for (var i = configurations.Count; i > 0; i--)
			{
				foreach (var indices in configurations.GetCombinations(i))
				{
					List<OrthogonalLine> intersection = null;

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

			throw new InvalidOperationException("There should always be at least one point in the intersection for shapes that may be neighbours in the layout.");
		}

		/// <summary>
		/// Gets configurations spaces of a given main configuration with respect to other configurations.
		/// </summary>
		/// <remarks>
		/// The main configuration will play a role of a shape that can be moved. Other configurations stays fixed.
		/// </remarks>
		/// <param name="mainConfiguration">Plays a role of a shape that can be moved.</param>
		/// <param name="configurations">Fixed configurations.</param>
		/// <returns></returns>
		protected abstract IList<Tuple<TConfiguration, ConfigurationSpace>> GetConfigurationSpaces(TConfiguration mainConfiguration, IList<TConfiguration> configurations);

		/// <summary>
		/// Gets a configuration space for given two configurations.
		/// </summary>
		/// <param name="mainConfiguration">Configuration with a shape that can be moved.</param>
		/// <param name="configuration">Configuration with a shape that is fixed.</param>
		/// <returns></returns>
		protected abstract ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configuration);

		/// <inheritdoc />
		public abstract ConfigurationSpace GetConfigurationSpace(TShapeContainer movingPolygon, TShapeContainer fixedPolygon);

		/// <inheritdoc />
		public abstract TShapeContainer GetRandomShape(TNode node);

		/// <inheritdoc />
		public abstract bool CanPerturbShape(TNode node);

		/// <inheritdoc />
		public abstract IReadOnlyCollection<TShapeContainer> GetShapesForNode(TNode node);

		/// <inheritdoc />
		public abstract IEnumerable<TShapeContainer> GetAllShapes();

		/// <inheritdoc />
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