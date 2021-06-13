using System;
using System.IO;
using Newtonsoft.Json;

namespace Edgar.Benchmarks
{
    public static class BenchmarkExtensions
    {
        public static void Save(this BenchmarkScenarioResult result, string name = null, string directory = "BenchmarkResults/", bool withDatetime = true)
        {
            var datetime = withDatetime ? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + "_" : "";
            var path = Path.Combine(directory, $"{datetime}{name ?? result.Name ?? string.Empty}.json");
            SaveResult(result, path);
        }

        private static void SaveResult(BenchmarkScenarioResult scenarioResult, string path)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));

            var json = JsonConvert.SerializeObject(scenarioResult, Formatting.Indented);
            var directory = Path.GetDirectoryName(path);

            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, json);
        }
    }
}