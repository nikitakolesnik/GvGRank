using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GvGRank_Server.Models;

namespace GvGRank_Server.Controllers
{
	[Route("api/VoteCount")]
	[ApiController]
	public class VoteCountController : ControllerBase
	{
		private readonly VoteDbContext _context;

		public VoteCountController(VoteDbContext context)
		{
			_context = context;
		}

		// GET: api/VoteCount
		[HttpGet]
		public int GetVotes()
		{
			return _context.Votes.Count();
		}
	}
}