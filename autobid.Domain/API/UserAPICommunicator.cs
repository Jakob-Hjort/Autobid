using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using autobid.Domain.Users;

namespace autobid.Domain.API;

public class UserAPICommunicator
{
    const string baseUrl = "localhost:5240/api/User";

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

        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<User?>()
            : null;
    }

    public async Task<bool> UpdatePasswordHash(int userId, string newPassword)
    {
        using HttpClient client = new();
        var response = await client.PutAsync($"{baseUrl}/UpdatePasswordHash?id={userId}&newPasswordHash={newPassword}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBalance(int userId, string balance)
    {
        using HttpClient client = new();
        var response = await client.PutAsync($"{baseUrl}/UpdateBalance?id={userId}&balance={balance}", null);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> DeleteUser(int userId)
    {
        using HttpClient client = new();
        var response = await client.DeleteAsync($"{baseUrl}/{userId}");
        return response.IsSuccessStatusCode;
    }
}
