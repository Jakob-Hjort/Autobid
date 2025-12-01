using System.Text.Json.Nodes;
using autobid.Domain.Auctions;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;
using autobid.Domain.Common.Enums;
using autobid.Domain;
class AuctionRequestBinder : IModelBinder
{
    
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string auctionJsonString = bindingContext.ValueProvider.GetValue("auction").FirstValue ?? "";
        string vehicleJsonString = bindingContext.ValueProvider.GetValue("vehicle").FirstValue ?? "";
        var minPrice = decimal.Parse(bindingContext.ValueProvider.GetValue("minPrice").FirstValue ?? "-1");
        var closeDate = DateTimeOffset.Parse(bindingContext.ValueProvider.GetValue("closeDate").FirstValue ?? "");
        var vehicleType = bindingContext.ValueProvider.GetValue("vehicleType").FirstValue;
        var userType = bindingContext.ValueProvider.GetValue("sellerType").FirstValue ?? "";
        string sellerJsonString = bindingContext.ValueProvider.GetValue("seller").FirstValue ?? "";

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        Dictionary<string, JsonNode?> vehicleData = JsonSerializer.Deserialize<Dictionary<string, JsonNode?>>(vehicleJsonString, options)!;
        Dictionary<string, JsonNode?> sellerData = JsonSerializer.Deserialize<Dictionary<string, JsonNode?>>(sellerJsonString, options)!;
        Vehicle vehicle = CarConverter.ConvertToCar(vehicleData, vehicleType!);
        User user = UserConverter.Convert(sellerData, userType);
        


        Auction auction = new(vehicle, user, minPrice, closeDate);
        bindingContext.Result = ModelBindingResult.Success(auction);
        await Task.CompletedTask;
    }
}