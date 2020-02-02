using Microsoft.AspNetCore.SignalR;

namespace GvGRank_Server.Hubs
{
	public class VoteHub : Hub
	{
		public void RecentVote(string[] vote)
		{
			Clients.All.SendAsync("recentvote", vote);
		}
	}
}