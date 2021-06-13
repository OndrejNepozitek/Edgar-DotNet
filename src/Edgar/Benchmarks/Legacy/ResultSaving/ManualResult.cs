using System;
using System.Collections.Generic;

namespace Edgar.Benchmarks.Legacy.ResultSaving
{
    public class ManualResult
    {
        public List<BenchmarkResult> InputResults { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public ManualResult(BenchmarkScenarioResult scenarioResult, ManualInfo manualInfo)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));
            if (manualInfo == null) throw new ArgumentNullException(nameof(manualInfo));

            InputResults = scenarioResult.Results;
            Name = manualInfo.Name ?? scenarioResult.Name ?? throw new ArgumentNullException(nameof(Name));
            Group = manualInfo.Group ?? throw new ArgumentNullException(nameof(Group));
        }
    }
}