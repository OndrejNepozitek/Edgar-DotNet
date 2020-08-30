using System;

namespace Edgar.Legacy.Benchmarks.ResultSaving
{
    public class UploadConfig
    {
        public string Url { get; }

        public UploadConfig(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}