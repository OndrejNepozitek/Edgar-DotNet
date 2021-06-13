namespace Edgar.Benchmarks.Legacy.ResultSaving
{
    public class CommitInfo
    {
        public string Commit { get; set; }

        public string CommitMessage { get; set; }

        public string Branch { get; set; }

        public string BuildNumber { get; set; }

        public string PullRequest { get; set; }
    }
}