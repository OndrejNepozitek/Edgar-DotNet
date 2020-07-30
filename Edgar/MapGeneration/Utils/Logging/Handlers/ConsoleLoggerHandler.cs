using System;

namespace MapGeneration.Utils.Logging.Handlers
{
    public class ConsoleLoggerHandler : ILoggerHandler
    {
        public void Write(string text)
        {
            Console.Write(text);
        }
    }
}