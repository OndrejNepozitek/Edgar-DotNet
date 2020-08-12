using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.ConfigurationSpaces.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.ConfigurationSpaces
{
    /// <inheritdoc cref="IConfigurationSpaces{TNode,TShape,TConfiguration,TConfigurationSpace}" />
	/// <summary>
	/// Abstract class for configuration spaces with common methods already implemented.
	/// </summary>
	public abstract class AbstractConfigurationSpaces<TNode, TShapeContainer, TConfiguration> : IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace>, IRandomInjectable
		where TConfiguration : IConfiguration<TShapeContainer, TNode>
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
		public Vector2Int GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			return GetRandomIntersectionPoint(mainConfiguration, configurations, out var _);
		}

		/// <inheritdoc />
		public Vector2Int GetRandomIntersectionPoint(TConfiguration mainConfiguration, IList<TConfiguration> configurations, out int configurationsSatisfied)
		{
			var intersection = GetMaximumIntersection(mainConfiguration, configurations, out configurationsSatisfied);

			if (configurationsSatisfied == 0)
			{
				return mainConfiguration.Position;
			}

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
		/// Throws when no intersection was found.
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

			configurationsSatisfied = 0;
			return null;
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
		public abstract ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configuration);

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

		private OrthogonalLine FastAddition(OrthogonalLine line, Vector2Int position) 
		{
			return new OrthogonalLine(line.From + position, line.To + position);
		}
	}
}