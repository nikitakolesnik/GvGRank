using GvGRank_Server.Exceptions;
using GvGRank_Server.Models;
using GvGRank_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GvGRank_Server.Controllers
{
	[Route("api/vote")]
	[ApiController]
	public class VoteController : ControllerBase
	{
		private readonly IVoteRepository _repo;

		public VoteController(IVoteRepository repo)
		{
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		// GET: api/Vote
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				return new OkObjectResult(await _repo.GetVotePairAsync(Request.HttpContext.Connection.RemoteIpAddress.ToString()));
			}
			catch (LowIntegrityVotingException)
			{
				return new OkObjectResult(new
				{
					name1 = "_",
					name2 = "You have voted too many times this minute, please try again shortly.",
					id1 = 0,
					id2 = 0
				});
			}
			catch (TooManyVotesException)
			{
				return new OkObjectResult(new
				{
					name1 = "_",
					name2 = "Your votes have triggered the troll alarm, please try again later.",
					id1 = 0,
					id2 = 0
				});
			}
			catch (UserMadeEveryVoteException)
			{
				return new OkObjectResult(new
				{
					name1 = "_",
					name2 = "You have voted on every possible combination. Now go take a walk, or call your parents.",
					id1 = 0,
					id2 = 0
				});
			}
		}



		// POST: api/Vote
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] VotePost v)
		{
			try
			{
				await _repo.SubmitVoteAsync(v, Request.HttpContext.Connection.RemoteIpAddress.ToString());
				return StatusCode(201);
			}
			catch (TooManyVotesException)
			{
				return StatusCode(403);
			}
			catch (LowIntegrityVotingException)
			{
				return StatusCode(403);
			}
			catch (InvalidVoteException)
			{
				return StatusCode(400);
			}
			catch (RepeatVoteException)
			{
				return StatusCode(403);
			}
		}
	}
}