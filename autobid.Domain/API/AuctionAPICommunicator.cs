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
    const string baseUrl = "http://localhost:5240/api/Auction";

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
        Type type = auction.Seller.GetType();        
        string userJson = "";
        if (auction.Seller is CorporateCustomer corporateCustomer)
            userJson = JsonSerializer.Serialize<CorporateCustomer>(corporateCustomer);
        else if (auction.Seller is PrivateCustomer privateCustomer)
            userJson = JsonSerializer.Serialize<PrivateCustomer>(privateCustomer);
        else
            throw new ArgumentException("unknown user type");
        string vehicleJson;
        if (auction.Vehicle is Truck truck)
            vehicleJson = JsonSerializer.Serialize<Truck>(truck);
        else if (auction.Vehicle is Bus bus)
            vehicleJson = JsonSerializer.Serialize<Bus>(bus);
        else if (auction.Vehicle is ProfessionalPersonalCar professionalPersonalCar)
            vehicleJson = JsonSerializer.Serialize<ProfessionalPersonalCar>(professionalPersonalCar);
        else if (auction.Vehicle is PrivatePersonalCar privatePersonalCar)
            vehicleJson = JsonSerializer.Serialize<PrivatePersonalCar>(privatePersonalCar);
        else
            throw new ArgumentException("unknown vehicle type");

        var body = new AuctionForAPI(auction.Id, vehicleJson, userJson, auction.MinimumPrice, auction.CloseDate,
            auction.Vehicle.GetType().Name, auction.Seller.GetType().Name);
        
        var response = await client.PostAsync($"{baseUrl}", 
            new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json"));
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
