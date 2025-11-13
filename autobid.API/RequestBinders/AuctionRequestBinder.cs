using autobid.Domain.Auctions;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata;

class AuctionRequestBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var userType = bindingContext.ValueProvider.GetValue("userType").FirstValue;
        var vehicleType = bindingContext.ValueProvider.GetValue("vehicleType").FirstValue;
        var vehicleId = bindingContext.ValueProvider.GetValue("id").FirstValue;

        Vehicle vehicle = vehicleType switch
        {
            "Truck" => new Truck(),
            _ => throw new ArgumentException("Invalid vehicle type")
        };

        User user = userType switch
        {
            "Private" => new PrivateCustomer(0, "", "", "", 0),
            _ => throw new ArgumentException("Invalid user type")
        };

        await Task.CompletedTask;
    }
}