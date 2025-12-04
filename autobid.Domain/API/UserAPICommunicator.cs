using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using autobid.Domain.Users;

namespace autobid.Domain.API;

public class UserAPICommunicator
{
    public const string BaseURL = "http://localhost:5240/api/User";
    readonly CommonApiCommunicatorModules _commonModules = new();
    public async Task<User?> Login(string username, string password)
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{BaseURL}/Login?username={username}&password={password}");
        User? user = null;
        if (!response.IsSuccessStatusCode)
            return null;

        try
        {
            user = await response.Content.ReadFromJsonAsync<PrivateCustomer?>();
        }
        catch
        {
            user = await response.Content.ReadFromJsonAsync<CorporateCustomer?>();
        }
        return user;
    }

    public async Task<User?> GetUserById(uint id)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.GetAsync($"{BaseURL}/{id}");

            return await _commonModules.ReadJsonIfSucces<User>(response);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdatePasswordHash(uint userId, string newPassword)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.PutAsync($"{BaseURL}/UpdatePassword?id={userId}&newPassword={newPassword}", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DoesUsernameExist(string username)
    {
        using HttpClient client = new();
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync($"{BaseURL}/DoesUsernameExist/{username}");
        }
        catch
        {
            return false;
        }

        bool res = await response.Content.ReadFromJsonAsync<bool>();
        return !response.IsSuccessStatusCode || res;

    }

    public async Task<bool> UpdateBalance(uint userId, decimal balance)
    {
        using HttpClient client = new();
        try
        {

            var response = await client.PutAsync($"{BaseURL}/UpdateBalance?id={userId}&balance={balance}", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteUser(uint userId)
    {
        using HttpClient client = new();
        try
        {

            var response = await client.DeleteAsync($"{BaseURL}/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CorporateCustomer?> CreateCorporateCustomer(CorporateCustomer user)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.PostAsJsonAsync($"{BaseURL}/CorporateCustomer", user);
            return await _commonModules.ReadJsonIfSucces<CorporateCustomer>(response);
        }
        catch
        {
            return null;
        }
    }

    public async Task<PrivateCustomer?> CreatePrivateCustomer(PrivateCustomer user)
    {
        using HttpClient client = new();
        HttpResponseMessage response;
        string ss = JsonSerializer.Serialize(user);
        try
        {
            response = await client.PostAsJsonAsync($"{BaseURL}/PrivateCustomer", user);
        }
        catch
        {
            return null;
        }
        return await _commonModules.ReadJsonIfSucces<PrivateCustomer>(response);
    }
}
