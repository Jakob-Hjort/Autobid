using autobid.Domain.Auctions;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata;

class AuctionRequestBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var closeDate = DateTimeOffset.Parse(bindingContext.ValueProvider.GetValue("closeDate").FirstValue ?? "");
        var vehicleType = bindingContext.ValueProvider.GetValue("Vehicle.vehicleType").FirstValue;
        var vehicleId = uint.Parse(bindingContext.ValueProvider.GetValue("vehicle.id").FirstValue ?? "0");
        var vehicleName = bindingContext.ValueProvider.GetValue("vehicle.name").FirstValue;
        var distanceTraveledKm = int.Parse(bindingContext.ValueProvider.GetValue("vehicle.distanceTraveledKm").FirstValue ?? "0");
        var regNumber = bindingContext.ValueProvider.GetValue("Vehicle.registrationNumber").FirstValue ?? "";
        var vehicleYear = int.Parse(bindingContext.ValueProvider.GetValue("Vehicle.year").FirstValue ?? "0");
        var engineLiters = double.Parse(bindingContext.ValueProvider.GetValue("Vehicle.engineLiters").FirstValue ?? "0.0");
        var hasTowHitch = bool.Parse(bindingContext.ValueProvider.GetValue("Vehicle.hasTowHitch").FirstValue ?? "0.0");
        var kmPerLiter = double.Parse(bindingContext.ValueProvider.GetValue("Vehicle.kmPerLiter").FirstValue ?? "0.0");

        Vehicle vehicle = vehicleType switch
        {
            "Truck" =>
                new Truck(vehicleId, vehicleName,
                distanceTraveledKm, regNumber, vehicleYear, engineLiters, hasTowHitch, kmPerLiter),
            "Bus" =>
                new Bus(vehicleId, vehicleName,
                distanceTraveledKm, regNumber, vehicleYear, engineLiters, hasTowHitch, kmPerLiter),
            "PrivatePersonalCar" =>
                new PrivatePersonalCar(vehicleId, vehicleName,
                distanceTraveledKm, regNumber, vehicleYear, engineLiters, hasTowHitch, kmPerLiter),
            "ProfessionalPersonalCar" =>
                new ProfessionalPersonalCar(vehicleId, vehicleName,
                distanceTraveledKm, regNumber, vehicleYear, engineLiters, hasTowHitch, kmPerLiter),
            _ => throw new ArgumentException("Invalid vehicle type")
        };
        var userType = bindingContext.ValueProvider.GetValue("Seller.userType").FirstValue;
        var userId = uint.Parse(bindingContext.ValueProvider.GetValue("Seller.Id").FirstValue ?? "0");
        var username = bindingContext.ValueProvider.GetValue("Seller.Username").FirstValue ?? "";
        var password = bindingContext.ValueProvider.GetValue("Seller.Password").FirstValue ?? "";
        var cpr = bindingContext.ValueProvider.GetValue("Seller.cpr").FirstValue ?? "";
        var cvr = bindingContext.ValueProvider.GetValue("Seller.cvr").FirstValue ?? "";
        var balance = decimal.Parse(bindingContext.ValueProvider.GetValue("Seller.Balance").FirstValue ?? "");



        User user = userType switch
        {
            "Private" => new PrivateCustomer(userId, username, password, cpr, balance),
            "Corporate" => new CorporateCustomer(userId, username, password, cvr, balance),
            _ => throw new ArgumentException("Invalid user type")
        };

        var minPrice = decimal.Parse(bindingContext.ValueProvider.GetValue("minPrice").FirstValue ?? "0");
        var auctionId = uint.Parse(bindingContext.ValueProvider.GetValue("Id").FirstValue ?? "0");


        Auction auction = new Auction(vehicle, user, minPrice, closeDate, auctionId);
        bindingContext.Result = ModelBindingResult.Success(auction);
        await Task.CompletedTask;
    }
}