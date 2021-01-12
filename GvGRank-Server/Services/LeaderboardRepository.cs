using GvGRank_Server.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GvGRank_Server.Services
{
	public class LeaderboardRepository : ILeaderboardRepository
	{
		private readonly VoteDbContext _context;
		private const int _minVotesToShow = 10;

		public LeaderboardRepository(VoteDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<object[]> GetLeaderboardAsync()
		{
			return await _context.Players
				.OrderByDescending(x => x.Rating)
				.Where(x => x.Wins + x.Losses >= _minVotesToShow && x.Active)
				.Select(x => new { x.Role, x.Name })
				.ToArrayAsync();
		}
	}
}
