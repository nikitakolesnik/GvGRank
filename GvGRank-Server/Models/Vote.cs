using System;
using System.ComponentModel.DataAnnotations;

namespace GvGRank_Server.Models
{
	public class Vote
	{
		// 24 bytes per row

		[Key]
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; set; } // 8 bytes? https://alexpinsker.blogspot.com/2011/10/what-is-size-of-datetime-type-in-c.html

		[Required]
		public int UserId { get; set; }

		[Required]
		public int WinId { get; set; }

		[Required]
		public int LoseId { get; set; }
	}
}
