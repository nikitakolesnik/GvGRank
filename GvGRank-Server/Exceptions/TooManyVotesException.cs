using System;

namespace GvGRank_Server.Exceptions
{
	public class TooManyVotesException : Exception
	{
		public TooManyVotesException()
		{
		}

		public TooManyVotesException(string message)
			: base(message)
		{
		}

		public TooManyVotesException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
