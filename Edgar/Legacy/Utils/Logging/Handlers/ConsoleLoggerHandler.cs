using System;

namespace Edgar.Legacy.Utils.Logging.Handlers
{
    public class ConsoleLoggerHandler : ILoggerHandler
    {
        public void Write(string text)
        {
            Console.Write(text);
        }
    }
}