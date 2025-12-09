using System;
using System.Data.Common;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using autobid.Domain.Auctions;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;

namespace autobid.Domain.API;

public class AuctionAPICommunicator
{
    const string baseUrl = $"http://localhost:5240/api/Auction";
    readonly CommonApiCommunicatorModules _commonModules = new();
    public async Task<Auction?> GetAuctionById(uint id)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.GetAsync($"{baseUrl}/{id}");
            
            var auctionForAPI = await _commonModules
                .ReadJsonIfSucces<AuctionForAPI>(response);
            return auctionForAPI?.ToAuction();    
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Auction>> GetAllAuctions()
    {
        using HttpClient client = new();
        try
        {
            var response = await client.GetAsync($"{baseUrl}");
            return await _commonModules.ReadJsonIfSucces<IEnumerable<Auction>>(response) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<Auction?> CreateAuction(Auction auction)
    {
        using HttpClient client = new();

        try
        {
            var body = AuctionForAPI.FromAuction(auction);
            var response = await client.PostAsJsonAsync($"{baseUrl}",
                body);
            return await _commonModules.ReadJsonIfSucces<Auction>(response);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Auction?> CloseAuction(Auction auction)
    {
        using HttpClient client = new();
        var response = await client.PutAsJsonAsync($"{baseUrl}/CloseAuction", auction);
        try
        {
            return await _commonModules.ReadJsonIfSucces<Auction>(response);
        }
        catch
        {
            return null;
        }
    }



    public async Task<IEnumerable<AuctionListItem>> GetAllAuctonOpenListItems()
    {
        using HttpClient client = new();
        try
        {
            var response = await client.GetAsync($"{baseUrl}/OpenListItems");
            return await response.Content.ReadFromJsonAsync<IEnumerable<AuctionListItem>>() ?? [];
        }
        catch
        {
            return [];
        }

    }

    public async Task<bool> CloseEndedAuctions()
    {
        using HttpClient client = new();
        try
        {
            var response = await client.PutAsync($"{baseUrl}/CloseEndedAuctions", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
