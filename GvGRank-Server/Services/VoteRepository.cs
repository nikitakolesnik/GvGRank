using GvGRank_Server.Context;
using GvGRank_Server.Entities;
using GvGRank_Server.Enums;
using GvGRank_Server.Exceptions;
using GvGRank_Server.Hubs;
using GvGRank_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GvGRank_Server.Services
{
	public class VoteRepository : IVoteRepository
	{
		private const int _ALLOWED_VOTES_PER_MINUTE = 30;
		private const int _ANTI_TAMPERING_THRESHOLD = 100;

		private const bool _ANTI_TROLL = true;
		private const bool _ANTI_FLOOD = true;
		private const bool _REAL_TIME_VOTES = true;

		private static Random _random = new Random();
		private readonly VoteDbContext _context;
		private IHubContext<VoteHub> _hub;

		public VoteRepository(VoteDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<string[][]> GetRecentVotesAsync(int count = 1, string player = "")
		{
			if (count < 1)
				return new string[][] { };
			else if (count > 100)
				count = 100;

			var result = _context.Votes
				.OrderByDescending(x => x.Date)
				.Join(_context.Players, v1 => v1.WinId, p1 => p1.Id, (v1, p1) => new { v1, p1 })
				.Join(_context.Players, v2 => v2.v1.LoseId, p2 => p2.Id, (v2, p2) => new { v2, p2 });

			// If a player name is provided
			if (!string.IsNullOrEmpty(player) /*&& _context.Players.Where(x => x.Name.Contains(player)).Any()*/)
				result = result.Where(p => p.v2.p1.Name.Contains(player) || p.p2.Name.Contains(player));

			return await result
				.Take(count)
				.Select(x => new string[] { x.v2.p1.Name, x.p2.Name })
				.ToArrayAsync();
		}

		public async Task<int> GetVoteCountAsync()
		{
			return await _context.Votes.CountAsync();
		}

		public async Task<object> GetVotePairAsync(string ip)
		{
			User user = await this.IdentifyUserAsync(ip); // Returns null if user has not voted before


			// Make sure the user is allowed to vote

			if (user != null)
			{
				if (user.VoteLimit >= _ALLOWED_VOTES_PER_MINUTE) // Voting too frequently
					throw new TooManyVotesException();

				if (user.AntiTamper >= _ANTI_TAMPERING_THRESHOLD) // Low integrity voting
					throw new LowIntegrityVotingException();
			}


			// Get all active players

			List<Player> activePlayers = await this.GetActivePlayersAsync();


			// Get all votes made on active players

			List<VoteSmall> usersActiveVotes = await
				(from v in _context.Votes
				 join p1 in activePlayers on v.WinId equals p1.Id
				 join p2 in activePlayers on v.LoseId equals p2.Id
				 where v.UserId == user.Id
					 && p1.Active
					 && p2.Active
				 select new VoteSmall()
				 {
					 WinId = v.WinId,
					 LoseId = v.LoseId,
					 Role = p1.Role
				 })
				.ToListAsync();

			int maxVotes = activePlayers.Count() - 1; // How many valid votes can exist per player


			// Refine list to players which the user hasn't made all possible votes on

			foreach (Player player in activePlayers)
			{
				int usersVotesForPlayer = usersActiveVotes
					.Where(x => (x.WinId == player.Id || x.LoseId == player.Id) && x.Role == player.Role) // Where the player was involved, and the opponent hasn't had their role changed
					.Count();

				if (usersVotesForPlayer == maxVotes) // If this player has had all vote combinations done for them
				{
					activePlayers.Remove(player); // Remove them from the list
				}
			}

			int playersToVoteOn = activePlayers.Count(); // count again, after having potentially removed players from the list


			// If user has already voted on everyone, return an error

			if (playersToVoteOn < 2) // can't make a match with 0 or 1 results
				throw new UserMadeEveryVoteException();


			// Select a random player to vote for

			Player player1 = activePlayers.Skip(_random.Next(playersToVoteOn - 1)).First();


			// Refine the remaining player list

			activePlayers.Remove(player1);                                   // Remove selected first player from the list
			activePlayers.RemoveAll(x => x.Role != player1.Role);            // Restrict remaining players to same role
			activePlayers.OrderBy(x => Math.Abs(player1.Rating - x.Rating)); // Sort by rating difference


			// Remove players which user has voted this player against

			int[] votedOnOpponents = usersActiveVotes
				.Where(x => x.WinId == player1.Id || x.LoseId == player1.Id) // If the player was involved
				.Select(x => (x.WinId == player1.Id) ? x.LoseId : x.WinId)   // Get the other player's ID
				.ToArray();

			activePlayers.RemoveAll(x => votedOnOpponents.Contains(x.Id));


			// Select player2 semi-randomly from remaining, with closer matches being higher weighted

			//		Generate a rating using Box-Muller transform https://stackoverflow.com/a/218600/10874809

			double u1 = 1.0 - _random.NextDouble();
			double u2 = 1.0 - _random.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
			double stddev = await this.GetMaxRatingDiffAsync(player1.Role) * 0.1 / 2.82;
			int result = Convert.ToInt32(stddev * randStdNormal); // Find player with rating difference nearest to this


			//		Find the closest player to this rating, and assign them to be the opponent

			int minDiff = int.MaxValue;
			Player player2 = activePlayers.First();

			foreach (Player player in activePlayers)
			{
				int ratingDiff = Math.Abs(player.Rating - (result + player1.Rating));

				if (ratingDiff < minDiff)
				{
					minDiff = ratingDiff;
					player2 = player;
				}
			}

			return new //VotePair
			{
				name1 = player1.Name,
				name2 = player2.Name,
				id1 = player1.Id,
				id2 = player2.Id
			};
		}

		public async Task SubmitVoteAsync(VotePost v, string ip)
		{
			User user = await IdentifyUserAsync(ip);


			// Make sure they're allowed to vote

			if (user.VoteLimit >= _ALLOWED_VOTES_PER_MINUTE) // Haven't voted more than X times per minute
			{
				throw new TooManyVotesException();
			}

			if (user.AntiTamper >= _ANTI_TAMPERING_THRESHOLD) // Haven't made too many "troll" votes
			{
				throw new LowIntegrityVotingException();
			}


			// Make sure the call isn't malformed

			if (v.WinId == v.LoseId)
			{
				throw new InvalidVoteException();
			}


			// Check if the player names are valid

			if (!_context.Players.Where(x => x.Id == v.WinId).Any()
				|| !_context.Players.Where(x => x.Id == v.LoseId).Any())
			{
				throw new InvalidVoteException();
			}


			// Disable vote flooding

			Vote latestVote = await _context.Votes.LastAsync();

			if (v.WinId == latestVote.WinId || _ANTI_FLOOD)
			{
				throw new TooManyVotesException();
			}


			// Make player instances

			Player playerWin = _context.Players.Where(x => x.Id == v.WinId).SingleOrDefault();
			Player playerLose = _context.Players.Where(x => x.Id == v.LoseId).SingleOrDefault();


			// Check if the player roles are valid

			if (playerWin.Role != playerLose.Role)
			{
				throw new InvalidVoteException();
			}


			// Check if the user has made this vote combination before

			bool hasUserMadeThisVote =
				(from votes in _context.Votes
				 where votes.UserId == user.Id
					 && votes.WinId == v.WinId
					 && votes.LoseId == v.LoseId
				 select votes)
				.Any();

			bool hasUserMadeOppositeVote =
				(from votes in _context.Votes
				 where votes.UserId == user.Id
					 && votes.WinId == v.LoseId
					 && votes.LoseId == v.WinId
				 select votes)
				.Any();

			if (hasUserMadeThisVote || hasUserMadeOppositeVote)
			{
				throw new RepeatVoteException();
			}


			// Prepare data

			_context.Update(playerWin);
			_context.Update(playerLose);
			_context.Update(user);


			// Update stats

			playerWin.Wins++;
			playerLose.Losses++;


			// Calculate change in player rating

			int ratingDiff = Math.Max(Math.Abs(playerWin.Rating - playerLose.Rating), 100);
			int maxRatingDiff = Math.Max((int)await this.GetMaxRatingDiffAsync(playerWin.Role), 100);
			double quotient = ratingDiff / maxRatingDiff;

			const double EULERS_NUMBER = 2.71828;
			const double MIN_CHANGE_PERCENT = 0.01;  // Minimum rating change for a comparison
			const double EVEN_CHANGE_PERCENT = 0.025; // Rating change for an even comparison
			const double MAX_CHANGE_PERCENT = 0.05;  // Maximum rating change for a comparison

			int ratingChange = 0;

			if (/* Unexpected result */ playerWin.Rating < playerLose.Rating)
			{
				ratingChange = Convert.ToInt32(ratingDiff * Math.Max(EVEN_CHANGE_PERCENT * Math.Pow(EULERS_NUMBER, 2 * Math.Log(MIN_CHANGE_PERCENT / EVEN_CHANGE_PERCENT) * quotient), MIN_CHANGE_PERCENT));
			}
			else // Expected result
			{
				ratingChange = Convert.ToInt32(ratingDiff * Math.Min(EVEN_CHANGE_PERCENT * Math.Pow(EULERS_NUMBER, 2 * Math.Log(MAX_CHANGE_PERCENT / EVEN_CHANGE_PERCENT) * quotient), MAX_CHANGE_PERCENT));
			}

			playerWin.Rating += ratingChange;
			playerLose.Rating -= ratingChange;


			// Add vote to database

			_context.Votes.Add(new Vote { Date = DateTime.Now, UserId = user.Id, WinId = v.WinId, LoseId = v.LoseId });


			// Make this vote show up in everyone's browser

			if (_REAL_TIME_VOTES)
			{
				await _hub.Clients.All.SendAsync("recentvote", new string[] { playerWin.Name, playerLose.Name });
			}


			// Increment user limits

			user.VoteLimit++;

			if (_ANTI_TROLL)
			{
				user.AntiTamper = Convert.ToInt32(user.AntiTamper * Math.Pow(4.64158883361, quotient));
			}


			// Done

			await _context.SaveChangesAsync();
		}

		[ResponseCache(Duration = 60)]
		private async Task<List<Player>> GetActivePlayersAsync()
		{
			return await _context.Players
				.Where(player => player.Active)
				.ToListAsync();
		}

		[ResponseCache(Duration = 5)]
		private async Task<int> GetMaxRatingDiffAsync(Role role)
		{
			int maxRating = await _context.Players.Where(x => x.Role == role).MaxAsync(x => x.Rating);
			int minRating = await _context.Players.Where(x => x.Role == role).MinAsync(x => x.Rating);

			return Math.Abs(maxRating - minRating);
		}

		private async Task<User> IdentifyUserAsync(string ip)
		{
			ip = this.GetStringSha256Hash(ip);

			if (_context.Users.Where(x => x.Ip == ip).Any()) // If this is a returning IP, fetch their ID
			{
				return await _context.Users.FirstOrDefaultAsync(x => x.Ip == ip);
			}
			else // Create as entry for this new user
			{
				User newUser = new User { Ip = ip };

				await _context.Users.AddAsync(newUser);
				await _context.SaveChangesAsync();

				return newUser;
			}
		}

		private string GetStringSha256Hash(string text) // Hiding the IP so that people don't get upset about me storing it. https://stackoverflow.com/a/21109622/10874809
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}

			using (System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed())
			{
				byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
				byte[] hash = sha.ComputeHash(textData);

				return BitConverter.ToString(hash).Replace("-", string.Empty);
			}
		}
	}
}
