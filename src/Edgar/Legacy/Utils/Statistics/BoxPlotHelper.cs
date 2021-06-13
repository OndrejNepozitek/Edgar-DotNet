using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Benchmarks;

namespace Edgar.Legacy.Utils.Statistics
{
    public class BoxPlotHelper
    {
        public static List<double> GetTimeDifferences(BenchmarkScenarioResult result, BenchmarkScenarioResult referenceResult, double successRateThreshold = 0d)
        {
            var times = result.Results.Select(x => x.Runs.Average(y => y.Time)).ToList();
            var timesReference = referenceResult.Results.Select(x => x.Runs.Average(y => y.Time)).ToList();

            for (var i = 0; i < result.Results.Count; i++)
            {
                var benchmarkResult = result.Results[i];
                var benchmarkResultReference = referenceResult.Results[i];

                var successRate = benchmarkResult.Runs.Count(x => x.IsSuccessful) / (double) benchmarkResult.Runs.Count;
                var successRateReference = benchmarkResultReference.Runs.Count(x => x.IsSuccessful) / (double) benchmarkResultReference.Runs.Count;

                if (successRate < successRateThreshold && successRateReference < successRateThreshold)
                {
                    times[i] = 1;
                    timesReference[i] = 1;
                }
            }

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