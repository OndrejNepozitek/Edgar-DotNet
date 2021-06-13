using System;

namespace Edgar.Benchmarks.Legacy.ResultSaving
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