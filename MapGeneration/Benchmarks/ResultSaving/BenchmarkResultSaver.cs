using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MapGeneration.Benchmarks.ResultSaving
{
    public class BenchmarkResultSaver
    {
        public void SaveResult(BenchmarkScenarioResult scenarioResult, string name = null, string directory = "BenchmarkResults/", bool withDatetime = true)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));

            var json = JsonConvert.SerializeObject(scenarioResult, Formatting.Indented);

            // TODO: ugly?
            var datetime = withDatetime ? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + "_" : "";
            // TODO: ugly? How to pass directory
            Directory.CreateDirectory(directory);
            File.WriteAllText($"{directory}{datetime}{name ?? scenarioResult.Name ?? string.Empty}.json", json);
        }

        public async Task UploadCommitResult(BenchmarkScenarioResult scenarioResult, UploadConfig config, CommitInfo commitInfo)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (commitInfo == null) throw new ArgumentNullException(nameof(commitInfo));

            await UploadResult(new CommitResult(scenarioResult, commitInfo), config);
        }

        public async Task UploadManualResult(BenchmarkScenarioResult scenarioResult, UploadConfig config, ManualInfo manualInfo)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (manualInfo == null) throw new ArgumentNullException(nameof(manualInfo));

            await UploadResult(new ManualResult(scenarioResult, manualInfo), config);
        }

        private async Task UploadResult<TResult>(TResult result, UploadConfig config)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(config.Url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}