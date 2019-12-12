using System;
using System.Collections.Generic;
using MapGeneration.Utils.Logging.Handlers;

namespace MapGeneration.Utils.Logging
{
    public class Logger
    {
        private readonly List<ILoggerHandler> loggerHandlers = new List<ILoggerHandler>();

        public Logger()
        {

        }

        public Logger(params ILoggerHandler[] loggerHandlers)
        {
            this.loggerHandlers.AddRange(loggerHandlers);
        }

        public void AddLoggerHandler(ILoggerHandler loggerHandler)
        {
            loggerHandlers.Add(loggerHandler);
        } 

        public void WriteLine(string text = "")
        {
            Write(text + Environment.NewLine);
        }

        public void Write(string text)
        {
            foreach (var loggerHandler in loggerHandlers)
            {
                loggerHandler.Write(text);
            }
        }
    }
}