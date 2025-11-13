using System;
using System.Net.Http.Json;
using autobid.Domain.Auctions;

namespace autobid.Domain.API;

public class AuctionAPICommunicator
{
    const string baseUrl = "localhost:5240/api/Auction";

    public async Task<Auction?> GetAuctionById(uint id)
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}/{id}");

        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Auction?>()
            : null;
    }

    public async Task<IEnumerable<Auction>> GetAllAuctions()
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Auction>>() ?? [];
        }

        return [];
    }

    public async Task<Auction?> CreateAuction(Auction auction)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}", auction);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Auction>();
        }

        return null;
    }

    public async Task<Auction?> CloseAuction(Auction auction)
    {
        using HttpClient client = new();
        var response = await client.PutAsJsonAsync($"{baseUrl}/CloseAuction", auction);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Auction>();
        }
        return null;
    }

    public async Task<bool> AddBid(Bid bid, uint auctionId)
    {
        using HttpClient client = new();
        var response = await client.PutAsJsonAsync($"{baseUrl}/AddBid?auctionId={auctionId}", bid);
        return response.IsSuccessStatusCode;
    }


    public async Task<IEnumerable<AuctionListItem>> GetAllAuctonOpenListItems()
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}/OpenListItems");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<AuctionListItem>>() ?? [];
        }

        return [];
    }

    public async Task<bool> CloseEndedAuctions()
    {
        using HttpClient client = new();
        var response = await client.PutAsync($"{baseUrl}/CloseEndedAuctions", null);
        return response.IsSuccessStatusCode; 
    }
}
