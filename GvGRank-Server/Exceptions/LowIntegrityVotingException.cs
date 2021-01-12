using System;

namespace GvGRank_Server.Exceptions
{
	public class LowIntegrityVotingException : Exception
	{
		public LowIntegrityVotingException()
		{
		}

		public LowIntegrityVotingException(string message)
			: base(message)
		{
		}

		public LowIntegrityVotingException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
