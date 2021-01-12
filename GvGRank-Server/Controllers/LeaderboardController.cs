using GvGRank_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GvGRank_Server.Controllers
{
	[Route("api/Leaderboard")]
	[ApiController]
	public class LeaderboardController : ControllerBase
	{
		private readonly ILeaderboardRepository _repo;

		public LeaderboardController(ILeaderboardRepository repo)
		{
			_repo = repo;
		}

		// GET api/leaderboard
		[HttpGet]
		public async Task<object[]> Get()
		{
			return await _repo.GetLeaderboardAsync();
		}
	}
}