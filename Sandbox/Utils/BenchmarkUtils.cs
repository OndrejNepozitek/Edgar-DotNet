using System;
using System.IO;
using System.Threading.Tasks;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.ResultSaving;

namespace Sandbox.Utils
{
    public static class BenchmarkUtils
    {
        public static async Task SaveAndUpload(this BenchmarkResultSaver resultSaver, BenchmarkScenarioResult scenarioResult, string name, string group)
        {
            if (resultSaver == null) throw new ArgumentNullException(nameof(resultSaver));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (group == null) throw new ArgumentNullException(nameof(group));

            resultSaver.SaveResult(scenarioResult, $"{group}_{name}");

            var uploadConfig = GetDefaultUploadConfig();
            await resultSaver.UploadManualResult(scenarioResult, uploadConfig, new ManualInfo()
            {
                Group = group,
                Name = name,
            });
        }

        private static UploadConfig GetDefaultUploadConfig()
        {
            var url = File.ReadAllText("manualResultUrl.txt");

            return new UploadConfig(url);
        }
    }
}