using GvGRank_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GvGRank_Server.Controllers
{
	[Route("api/VoteCount")]
	[ApiController]
	public class VoteCountController : ControllerBase
	{
		private readonly IVoteRepository _repo;

		public VoteCountController(IVoteRepository repo)
		{
			_repo = repo ?? throw new System.ArgumentNullException(nameof(repo));
		}

		// GET: api/VoteCount
		[HttpGet]
		public async Task<int> Get()
		{
			return await _repo.GetVoteCountAsync();
		}
	}
}