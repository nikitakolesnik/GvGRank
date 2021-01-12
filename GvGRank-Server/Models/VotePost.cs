namespace GvGRank_Server.Models
{
	public class VotePost
	{
		// A condensed & expanded version of Vote
		// Used in VoteController.Post to capture client output

		public int WinId { get; set; }
		public int LoseId { get; set; }
	}
}
