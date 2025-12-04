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
    readonly CommonApiCommunicatorModules _commonModules = new();
    public async Task<Auction?> GetAuctionById(uint id)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.GetAsync($"{baseUrl}/{id}");
            
            var auctionForAPI = await _commonModules
                .ReadJsonIfSucces<AuctionForAPI>(response);
            User? user = auctionForAPI?.SellerType switch
            {
                "PrivateCustomer" => JsonSerializer
                    .Deserialize<PrivateCustomer>(auctionForAPI.SellerJson)!,
                "CorporateCustomer" => JsonSerializer
                    .Deserialize<CorporateCustomer>(auctionForAPI.SellerJson)!,
                _ => throw new Exception("unknown user type")
            };

            Vehicle? vehicle = auctionForAPI.VehicleType switch
            {
                "PrivatePersonalCar" => JsonSerializer
                    .Deserialize<PrivatePersonalCar>(auctionForAPI.VehicleJson)!,
                "ProfessionalPersonalCar" => JsonSerializer
                    .Deserialize<ProfessionalPersonalCar>(auctionForAPI.VehicleJson)!,
                "Truck" => JsonSerializer.Deserialize<Truck>(auctionForAPI.VehicleJson)!,
                "Bus" => JsonSerializer.Deserialize<Bus>(auctionForAPI.VehicleJson)!,
                _ => throw new Exception("unknown vehicle type")
            };

            return new Auction(vehicle, user,
                auctionForAPI.MinPrice, auctionForAPI.CloseDate);
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
        {
            vehicleJson = JsonSerializer.Serialize<Truck>(truck);
        }
        else if (auction.Vehicle is Bus bus)
        {
            vehicleJson = JsonSerializer.Serialize<Bus>(bus);
        }
        else if (auction.Vehicle is ProfessionalPersonalCar professionalPersonalCar)
        {
            vehicleJson = JsonSerializer.Serialize<ProfessionalPersonalCar>(professionalPersonalCar);
        }
        else if (auction.Vehicle is PrivatePersonalCar privatePersonalCar)
        {
            vehicleJson = JsonSerializer.Serialize<PrivatePersonalCar>(privatePersonalCar);
        }
        else
        {
            throw new ArgumentException("unknown vehicle type");
        }

        var body = new AuctionForAPI(auction.Id, vehicleJson, userJson, auction.MinimumPrice, auction.CloseDate,
            auction.Vehicle.GetType().Name, auction.Seller.GetType().Name);

        try
        {

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

    public async Task<bool> AddBid(Bid bid, uint auctionId)
    {
        using HttpClient client = new();
        try
        {

            var response = await client.PutAsJsonAsync($"{baseUrl}/AddBid?auctionId={auctionId}", bid);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
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
