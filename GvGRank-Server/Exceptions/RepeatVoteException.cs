using System;

namespace GvGRank_Server.Exceptions
{
	public class RepeatVoteException : Exception
	{
		public RepeatVoteException()
		{
		}

		public RepeatVoteException(string message)
			: base(message)
		{
		}

		public RepeatVoteException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
