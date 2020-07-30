using System;

namespace MapGeneration.Utils
{
    public static class FileNamesHelper
    {
        public static string PrefixWithTimestamp(string text)
        {
            return $"{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}_{text}";
        }
    }
}