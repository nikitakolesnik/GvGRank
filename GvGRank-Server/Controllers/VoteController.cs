using GvGRank_Server.Hubs;
using GvGRank_Server.Models;
using GvGRank_Server.Models.UtilityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GvGRank_Server.Controllers
{
	[Route("api/vote")]
	[ApiController]
	public class VoteController : ControllerBase
	{
		private const int ALLOWED_VOTES_PER_MINUTE = 30;
		private const int ANTI_TAMPERING_THRESHOLD = 100;

		private const bool ANTI_TROLL = true;
		private const bool ANTI_FLOOD = true;
		private const bool REAL_TIME_VOTES = true;

		private VoteDbContext _context;
		private IHubContext<VoteHub> _hub;
		private static Random _random = new Random();

		public VoteController(VoteDbContext context, IHubContext<VoteHub> hub)
		{
			_context = context;
			_hub = hub;
		}



		// GET: api/Vote
		[HttpGet]
		public IActionResult Get()
		{
			User user = IdentifyUser(); // Returns null if user has not voted before


			// Make sure the user is allowed to vote

			if (user != null)
			{

				if (user.VoteLimit >= ALLOWED_VOTES_PER_MINUTE) // Voting too frequently
				{
					return new OkObjectResult(new 
					{ 
						name1 = "_", 
						name2 = "You have voted too many times this minute, please try again shortly.", 
						id1 = 0, 
						id2 = 0 
					});
				}

				if (user.AntiTamper >= ANTI_TAMPERING_THRESHOLD) // Low integrity voting
				{
					return new OkObjectResult(new 
					{ 
						name1 = "_", 
						name2 = "Your votes have triggered the troll alarm, please try again later.", 
						id1 = 0, 
						id2 = 0 
					});
				}
			}


			// Get all active players

			List<Player> activePlayers = GetActivePlayers();


			// Get all votes made on active players

			List<VoteSmall> usersActiveVotes = 
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
				.ToList();

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
			{
				return new OkObjectResult(new 
				{ 
					name1 = "_", 
					name2 = "You have voted on every possible combination. Now go take a walk, or call your parents.", 
					id1 = 0, 
					id2 = 0 
				});
			}


			// Select a random player to vote for

			Player player1 = activePlayers.Skip(_random.Next(playersToVoteOn - 1)).First();


			// Refine the remaining player list

			activePlayers.Remove(player1);                                   // Remove selected first player from the list
			activePlayers.RemoveAll(x => x.Role != player1.Role);            // Restrict remaining players to same role
			activePlayers.OrderBy(x => Math.Abs(player1.Shitlo - x.Shitlo)); // Sort by rating difference


			// Remove players which user has voted this player against

			int[] votedOnOpponents = usersActiveVotes
				.Where(x => x.WinId == player1.Id || x.LoseId == player1.Id) // If the player was involved
				.Select(x => (x.WinId == player1.Id) ? x.LoseId : x.WinId)   // Get the other player's ID
				.ToArray();

			activePlayers.RemoveAll(x => votedOnOpponents.Contains(x.Id));


			// Select player2 semi-randomly from remaining, with closer matches being higher weighted
			
			//		Generate a rating using Box-Muller transform https://stackoverflow.com/a/218600/10874809

			double u1            = 1.0 - _random.NextDouble();
			double u2            = 1.0 - _random.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
			double stddev        = GetMaxRatingDiff(player1.Role) * 0.1 / 2.82;
			int    result        = Convert.ToInt32(stddev * randStdNormal); // Find player with rating difference nearest to this


			//		Find the closest player to this rating, and assign them to be the opponent

			int minDiff = int.MaxValue;
			Player player2 = activePlayers.First();

			foreach (Player player in activePlayers)
			{
				int ratingDiff = Math.Abs(player.Shitlo - (result + player1.Shitlo));

				if (ratingDiff < minDiff)
				{
					minDiff = ratingDiff;
					player2 = player;
				}
			}

			return new OkObjectResult(new
			{
				name1 = player1.Name,
				name2 = player2.Name,
				id1   = player1.Id,
				id2   = player2.Id
			});
		}



		// POST: api/Vote
		public IActionResult Post([FromBody]VotePost v)
		{
			User user = IdentifyUser();


			// Make sure they're allowed to vote

			if (user.VoteLimit >= ALLOWED_VOTES_PER_MINUTE      // Haven't voted more than X times per minute
				|| user.AntiTamper >= ANTI_TAMPERING_THRESHOLD) // Haven't made too many "troll" votes
			{
				return StatusCode(403);
			}


			// Make sure the call isn't malformed

			if (!ModelState.IsValid || v.WinId == v.LoseId)
			{
				return StatusCode(400);
			}


			// Check if the player names are valid

			if (!_context.Players.Where(x => x.Id == v.WinId).Any()
				|| !_context.Players.Where(x => x.Id == v.LoseId).Any())
			{
				return StatusCode(400);
			}


			// Disable vote flooding

			Vote latestVote = _context.Votes.Last();

			if (v.WinId == latestVote.WinId || ANTI_FLOOD)
			{
				return StatusCode(400);
			}


			// Make player instances

			Player playerWin  = _context.Players.Where(x => x.Id == v.WinId).SingleOrDefault();
			Player playerLose = _context.Players.Where(x => x.Id == v.LoseId).SingleOrDefault();


			// Check if the player roles are valid

			if (playerWin.Role != playerLose.Role)
			{
				return StatusCode(400);
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
				return StatusCode(403);
			}


			// Prepare data

			_context.Update(playerWin);
			_context.Update(playerLose);
			_context.Update(user);


			// Update stats

			playerWin.Wins++;
			playerLose.Losses++;


			// Calculate change in player rating

			int    ratingDiff    = Math.Max(Math.Abs(playerWin.Shitlo - playerLose.Shitlo), 100);
			int    maxRatingDiff = Math.Max(GetMaxRatingDiff(playerWin.Role), 100);
			double quotient      = ratingDiff / maxRatingDiff;

			const double EULERS_NUMBER       = 2.71828;
			const double MIN_CHANGE_PERCENT  = 0.01;  // Minimum rating change for a comparison
			const double EVEN_CHANGE_PERCENT = 0.025; // Rating change for an even comparison
			const double MAX_CHANGE_PERCENT  = 0.05;  // Maximum rating change for a comparison

			int ratingChange = 0;

			if (/* Unexpected result */ playerWin.Shitlo < playerLose.Shitlo)
			{
				ratingChange = Convert.ToInt32(ratingDiff * Math.Max(EVEN_CHANGE_PERCENT * Math.Pow(EULERS_NUMBER, 2 * Math.Log(MIN_CHANGE_PERCENT / EVEN_CHANGE_PERCENT) * quotient), MIN_CHANGE_PERCENT));
			}
			else // Expected result
			{
				ratingChange = Convert.ToInt32(ratingDiff * Math.Min(EVEN_CHANGE_PERCENT * Math.Pow(EULERS_NUMBER, 2 * Math.Log(MAX_CHANGE_PERCENT / EVEN_CHANGE_PERCENT) * quotient), MAX_CHANGE_PERCENT));
			}

			playerWin.Shitlo += ratingChange;
			playerLose.Shitlo -= ratingChange;


			// Add vote to database

			_context.Votes.Add(new Vote { Date = DateTime.Now, UserId = user.Id, WinId = v.WinId, LoseId = v.LoseId });


			// Make this vote show up in everyone's browser

			if (REAL_TIME_VOTES)
			{
				_hub.Clients.All.SendAsync("recentvote", new string[] { playerWin.Name, playerLose.Name });
			}


			// Increment user limits

			user.VoteLimit++;

			if (ANTI_TROLL)
			{
				user.AntiTamper = Convert.ToInt32(user.AntiTamper * Math.Pow(4.64158883361, quotient));
			}


			// Done

			_context.SaveChanges();

			return StatusCode(201);
		}



		[ResponseCache(Duration = 60)]
		private List<Player> GetActivePlayers()
		{
			return _context.Players
				.Where(player => player.Active)
				.ToList();
		}



		[ResponseCache(Duration = 5)]
		private int GetMaxRatingDiff(int role)
		{
			int maxRating = _context.Players.Where(x => x.Role == role).Max(x => x.Shitlo);
			int minRating = _context.Players.Where(x => x.Role == role).Min(x => x.Shitlo);

			return Math.Abs(maxRating - minRating);
		}



		private User IdentifyUser()
		{
			string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

			ip = GetStringSha256Hash(ip);

			if (_context.Users.Where(x => x.Ip == ip).Any()) // If this is a returning IP, fetch their ID
			{
				return _context.Users.Where(x => x.Ip == ip).FirstOrDefault();
			}
			else // Create as entry for this new user
			{
				User newUser = new User { Ip = ip };

				_context.Users.Add(newUser);
				_context.SaveChanges();

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
				byte[] hash     = sha.ComputeHash(textData);

				return BitConverter.ToString(hash).Replace("-", string.Empty);
			}
		}
	}
}