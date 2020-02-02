namespace GvGRank_Server.Models.UtilityModels
{
	public class VoteSmall
	{
		// A condensed & expanded version of Vote with a property from Player
		// Used in VoteController.Get to avoid saving uneccessary values

		public int WinId { get; set; }
		public int LoseId { get; set; }
		public int Role { get; set; } = 0;
	}
}
