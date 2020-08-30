using System;
using System.Collections.Generic;

namespace Edgar.Legacy.Benchmarks.ResultSaving
{
    internal class CommitResult
    {
        public List<BenchmarkResult> InputResults { get; set; }

        public string Commit { get; set; }

        public string CommitMessage { get; set; }

        public string Branch { get; set; }

        public string BuildNumber { get; set; }

        public string PullRequest { get; set; }

        public CommitResult(BenchmarkScenarioResult scenarioResult, CommitInfo commitInfo)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));
            if (commitInfo == null) throw new ArgumentNullException(nameof(commitInfo));

            InputResults = scenarioResult.BenchmarkResults;
            Commit = commitInfo.Commit;
            CommitMessage = commitInfo.CommitMessage;
            Branch = commitInfo.Branch;
            BuildNumber = commitInfo.BuildNumber;
            PullRequest = commitInfo.PullRequest;
        }
    }
}