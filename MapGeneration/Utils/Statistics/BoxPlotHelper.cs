using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Benchmarks;

namespace MapGeneration.Utils.Statistics
{
    public class BoxPlotHelper
    {
        public static List<double> GetTimeDifferences(BenchmarkScenarioResult result, BenchmarkScenarioResult referenceResult)
        {
            var times = result.BenchmarkResults.Select(x => x.Runs.Average(y => y.Time)).ToList();
            var timesReference = referenceResult.BenchmarkResults.Select(x => x.Runs.Average(y => y.Time)).ToList();
            var differences = times.Zip(timesReference, StatisticsUtils.DifferenceToReference).ToList();

            return differences;
        }

        public static BoxPlotValues GetBoxPlotValues(BenchmarkScenarioResult result, BenchmarkScenarioResult referenceResult, bool excludeOutliers = true)
        {
            var timeDifferences = GetTimeDifferences(result, referenceResult);
            return GetBoxPlotValues(timeDifferences, excludeOutliers);
        }

        public static BoxPlotValues GetBoxPlotValues(IEnumerable<double> values, bool excludeOutliers = true)
        {
            var orderedValues = values.ToList();
            orderedValues.Sort();

            var median = orderedValues.GetMedian();
            var q1 = orderedValues[(int) Math.Floor(orderedValues.Count * 1 / 4d)];
            var q3 = orderedValues[(int) Math.Floor(orderedValues.Count * 3 / 4d)];
            var minimum = orderedValues[0];
            var maximum = orderedValues.Last();

            if (excludeOutliers)
            {
                var iqr = q3 - q1;
                var lowerBound = q1 - 1.5 * iqr;
                var uppedBound = q3 + 1.5 * iqr;

                minimum = orderedValues.First(x => x >= lowerBound);
                maximum = orderedValues.Last(x => x <= uppedBound);
            }

            return new BoxPlotValues()
            {
                Minimum = minimum,
                Q1 = q1,
                Median = median,
                Q3 = q3,
                Maximum = maximum,
            };
        }
    }
}