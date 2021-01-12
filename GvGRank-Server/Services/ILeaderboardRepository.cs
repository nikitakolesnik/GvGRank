using System.Threading.Tasks;

namespace GvGRank_Server.Services
{
	public interface ILeaderboardRepository
	{
		Task<object[]> GetLeaderboardAsync();
	}
}
