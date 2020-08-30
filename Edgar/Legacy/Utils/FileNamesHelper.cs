using System;

namespace Edgar.Legacy.Utils
{
    public static class FileNamesHelper
    {
        public static string PrefixWithTimestamp(string text)
        {
            return $"{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}_{text}";
        }
    }
}