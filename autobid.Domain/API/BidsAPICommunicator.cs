using System;
using System.Net.Http.Json;
using autobid.Domain.Auctions;

namespace autobid.Domain.API;

public class BidsAPICommunicator
{
    const string baseUrl = $"http://localhost:5240/api/Bid";
    public async Task<bool> AddBid(Bid bid, uint auctionId)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.PostAsJsonAsync($"{baseUrl}", BidForApi.FromBid(bid));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
