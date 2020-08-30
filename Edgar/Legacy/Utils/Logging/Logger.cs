using System;
using System.Collections.Generic;
using Edgar.Legacy.Utils.Logging.Handlers;

namespace Edgar.Legacy.Utils.Logging
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

        public void WriteLine(object o)
        {
            Write(o.ToString() + Environment.NewLine);
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