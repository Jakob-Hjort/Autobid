using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using autobid.Domain.Users;

namespace autobid.Domain.API;

public class UserAPICommunicator
{
    const string baseUrl = "localhost:5240/api/User";
    readonly CommonApiCommunicatorModules _commonModules = new();
    public async Task<User?> Login(string username, string password)
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}/Login?username={username}&password={password}");

        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<User?>()
            : null;
    }

    public async Task<User?> GetUserById(int id)
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}/{id}");

        return await _commonModules.ReadIfSucces<User>(response);
    }

    public async Task<bool> UpdatePasswordHash(uint userId, string newPassword)
    {
        using HttpClient client = new();
        var response = await client.PutAsync($"{baseUrl}/UpdatePasswordHash?id={userId}&newPasswordHash={newPassword}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DoesUsernameExist()
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}/");
        return response.IsSuccessStatusCode &&
            await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<bool> UpdateBalance(uint userId, decimal balance)
    {
        using HttpClient client = new();
        var response = await client.PutAsync($"{baseUrl}/UpdateBalance?id={userId}&balance={balance}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUser(uint userId)
    {
        using HttpClient client = new();
        var response = await client.DeleteAsync($"{baseUrl}/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<CorporateCustomer?> CreateCorporateCustomer(CorporateCustomer user)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}/CorporateCustomer", user);
        return await _commonModules.ReadIfSucces<CorporateCustomer>(response);
    }

    public async Task<PrivateCustomer?> CreatePrivateCustomer(PrivateCustomer user)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}/PrivateCustomer", user);
        return await _commonModules.ReadIfSucces<PrivateCustomer>(response);
    }
    

}
