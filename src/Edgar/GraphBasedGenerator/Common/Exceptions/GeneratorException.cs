using System;

namespace Edgar.GraphBasedGenerator.Common.Exceptions
{
    /// <summary>
    /// Base exception of the level generator.
    /// </summary>
    public class GeneratorException : Exception
    {
        public GeneratorException(string message) : base(message)
        {
            /* empty */
        }
    }
}