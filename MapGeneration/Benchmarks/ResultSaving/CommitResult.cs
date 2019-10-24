using System;
using System.Collections.Generic;

namespace MapGeneration.Benchmarks.ResultSaving
{
    internal class CommitResult
    {
        public List<BenchmarkScenarioResult.InputResult> InputResults { get; set; }

        public string Commit { get; set; }

        public string CommitMessage { get; set; }

        public string Branch { get; set; }

        public string BuildNumber { get; set; }

        public string PullRequest { get; set; }

        public CommitResult(BenchmarkScenarioResult scenarioResult, CommitInfo commitInfo)
        {
            if (scenarioResult == null) throw new ArgumentNullException(nameof(scenarioResult));
            if (commitInfo == null) throw new ArgumentNullException(nameof(commitInfo));

            InputResults = scenarioResult.InputResults;
            Commit = commitInfo.Commit;
            CommitMessage = commitInfo.CommitMessage;
            Branch = commitInfo.Branch;
            BuildNumber = commitInfo.BuildNumber;
            PullRequest = commitInfo.PullRequest;
        }
    }
}