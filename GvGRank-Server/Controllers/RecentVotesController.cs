using GvGRank_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GvGRank_Server.Controllers
{
	[Route("api/RecentVotes")]
	[ApiController]
	public class RecentVotesController : ControllerBase
	{
		private readonly IVoteRepository _repo;

		public RecentVotesController(IVoteRepository repo)
		{
			_repo = repo ?? throw new System.ArgumentNullException(nameof(repo));
		}

		// GET: api/Votes
		[HttpGet]
		public async Task<string[][]> Get(int count = 1, string player = "")
		{
			return await _repo.GetRecentVotesAsync(count, player);
		}
	}
}