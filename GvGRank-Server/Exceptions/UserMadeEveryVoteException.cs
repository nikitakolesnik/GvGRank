using System;

namespace GvGRank_Server.Exceptions
{
	public class UserMadeEveryVoteException : Exception
	{
		public UserMadeEveryVoteException()
		{
		}

		public UserMadeEveryVoteException(string message)
			: base(message)
		{
		}

		public UserMadeEveryVoteException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
