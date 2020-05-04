using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Simplified.ConfigurationSpaces
{
    public abstract class ConfigurationSpacesBase<TConfiguration> : IConfigurationSpaces<TConfiguration, ConfigurationSpace>, IRandomInjectable
        where TConfiguration : IPositionConfiguration
    {
        protected Random Random;
        protected ILineIntersection<OrthogonalLine> LineIntersection;

        protected ConfigurationSpacesBase(ILineIntersection<OrthogonalLine> lineIntersection)
        {
            LineIntersection = lineIntersection;
        }

        /// <inheritdoc />
        public void InjectRandomGenerator(Random random)
        {
            Random = random;
        }

        public abstract List<OrthogonalLine> GetMaximumIntersection(TConfiguration mainConfiguration,
            List<TConfiguration> configurations, int minimumSatisfiedConfigurations,
            out int configurationsSatisfied);

        public abstract ConfigurationSpace GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2);

        public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
        {
            var space = GetConfigurationSpace(configuration1, configuration2);
            var lines1 = new List<OrthogonalLine>() {new OrthogonalLine(configuration1.Position, configuration1.Position)};

            return LineIntersection.DoIntersect(space.Lines.Select(x => FastAddition(x, configuration2.Position)), lines1);
        }

        protected List<OrthogonalLine> GetMaximumIntersection(List<Tuple<TConfiguration, ConfigurationSpace>> configurationSpaces, int minimumSatisfiedConfigurations, out int configurationsSatisfied)
        {
            configurationSpaces.Shuffle(Random);

            for (var size = configurationSpaces.Count; size > 0; size--)
            {
                if (size < minimumSatisfiedConfigurations)
                {
                    break;
                }

                foreach (var indices in configurationSpaces.GetCombinations(size))
                {
                    List<OrthogonalLine> intersection = null;

                    foreach (var index in indices)
                    {
                        var linesToIntersect = configurationSpaces[index]
                            .Item2
                            .Lines
                            .Select(x => x + configurationSpaces[index].Item1.Position)
                            .ToList();
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

        private OrthogonalLine FastAddition(OrthogonalLine line, IntVector2 position) 
        {
            return new OrthogonalLine(line.From + position, line.To + position);
        }
    }
}