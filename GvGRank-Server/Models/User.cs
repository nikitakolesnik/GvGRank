using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GvGRank_Server.Models
{
	public class User
	{
		// 152 bytes per row

		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(64)] // SHA256 hash of IP address
		public string Ip { get; set; }

		[DefaultValue(0)]
		public int VoteLimit { get; set; } = 0;

		[DefaultValue(1)]
		public int AntiTamper { get; set; } = 1;
	}
}
