using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GvGRank_Server.Models;

namespace GvGRank_Server.Controllers
{
	[Route("api/RecentVotes")]
	[ApiController]
	public class RecentVotesController : ControllerBase
	{
		private VoteDbContext _context;

		public RecentVotesController(VoteDbContext context)
		{
			_context = context;
		}

		// GET: api/Votes
		[HttpGet]
		public string[][] Get(int count = 1, string player = "")
		{
			if (count < 1)
			{
				return new string[][] { };
			}

			if (count > 100)
			{
				count = 100;
			}

			// Code gore - since it's a query with a conditional clause, it needs to be written with lambda syntax. Very messy with a double same-table join.
			var result = _context.Votes
				.OrderByDescending(x => x.Date)
				.Join(_context.Players, v1 => v1.WinId,     p1 => p1.Id, (v1, p1) => new { v1, p1 })
				.Join(_context.Players, v2 => v2.v1.LoseId, p2 => p2.Id, (v2, p2) => new { v2, p2 });

			// If a player name is provided
			if (!string.IsNullOrEmpty(player) /*&& _context.Players.Where(x => x.Name.Contains(player)).Any()*/)
				result = result.Where(p => p.v2.p1.Name.Contains(player) || p.p2.Name.Contains(player));

			return result
				.Take(count)
				.Select(x => new string[] { x.v2.p1.Name, x.p2.Name })
				.ToArray();
		}
	}
}