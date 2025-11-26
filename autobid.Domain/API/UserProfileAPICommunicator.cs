using System;
using autobid.Domain.Users;

namespace autobid.Domain.API;

public class UserProfileAPICommunicator : IUserProfileReadService
{
    CommonApiCommunicatorModules _commonModules = new();
    public async Task<UserProfileSummary> GetAsync(uint userId)
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{UserAPICommunicator.BaseURL}/UserProfileSummary/{userId}");
        return await _commonModules.ReadJsonIfSucces<UserProfileSummary>(response) 
            ?? throw new Exception("Invalid UserProfile from API");
    }
}
