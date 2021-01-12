using System;

namespace GvGRank_Server.Exceptions
{
	public class InvalidVoteException : Exception
	{
		public InvalidVoteException()
		{
		}

		public InvalidVoteException(string message)
			: base(message)
		{
		}

		public InvalidVoteException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
