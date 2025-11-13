using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace autobid.Domain.API;

public class CommonApiCommunicatorModules
{
    public async Task<T?> ReadJsonIfSucces<T>(HttpResponseMessage response, T? defaultReturn = default)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        return defaultReturn;
    }
}
