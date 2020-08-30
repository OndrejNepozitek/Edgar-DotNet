using System;

namespace Edgar.Legacy.Utils.ConfigParsing
{
    /// <summary>
	/// Exception to be thrown when parsing yaml configs.
	/// </summary>
	public class ParsingException : Exception
	{
		public ParsingException()
		{
		}

		public ParsingException(string message)
			: base(message)
		{
		}

		public ParsingException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}