using GvGRank_Server.Models;
using System.Threading.Tasks;

namespace GvGRank_Server.Services
{
	public interface IVoteRepository
	{
		Task<string[][]> GetRecentVotesAsync(int count, string player);
		Task<int> GetVoteCountAsync();
		Task<object> GetVotePairAsync(string ip);
		Task SubmitVoteAsync(VotePost v, string ip);
	}
}
