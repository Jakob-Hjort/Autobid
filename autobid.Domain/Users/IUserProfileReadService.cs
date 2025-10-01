// autobid.Domain/Users/IUserProfileReadService.cs
using System.Threading.Tasks;

namespace autobid.Domain.Users;
public interface IUserProfileReadService
{
    Task<UserProfileSummary> GetAsync(uint userId);
}

public sealed record UserProfileSummary(
    uint UserId, string Username, decimal Balance, int AuctionCount, int AuctionsWon
);
