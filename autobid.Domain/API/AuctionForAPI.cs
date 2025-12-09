using System.Text.Json;
using autobid.Domain.Auctions;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;

namespace autobid.Domain.API;

public record class AuctionForAPI(uint Id, string VehicleJson, string SellerJson, decimal MinPrice,
    DateTimeOffset CloseDate, VehicleTypes VehicleType, UserTypes SellerType, Bid[]? Bids = null)
{
    public Auction ToAuction()
    {
        User? user = SellerType switch
        {
            UserTypes.PrivateCustomer => JsonSerializer
                .Deserialize<PrivateCustomer>(SellerJson)!,
            UserTypes.CorporateCustomer => JsonSerializer
                .Deserialize<CorporateCustomer>(SellerJson)!,
            _ => throw new Exception("unknown user type")
        };

        Vehicle? vehicle = VehicleType switch
        {
            VehicleTypes.PrivatePersonalCar => JsonSerializer
                .Deserialize<PrivatePersonalCar>(VehicleJson)!,
            VehicleTypes.ProfessionalPersonalCar => JsonSerializer
                    .Deserialize<ProfessionalPersonalCar>(VehicleJson)!,
            VehicleTypes.Truck => JsonSerializer.Deserialize<Truck>(VehicleJson)!,
            VehicleTypes.Bus => JsonSerializer.Deserialize<Bus>(VehicleJson)!,
            _ => throw new Exception("unknown vehicle type")
        };

        return new Auction(vehicle, user,
            MinPrice, CloseDate, Id);
    }

    public static AuctionForAPI FromAuction(Auction auction)
    {
        UserTypes sellerType;
        string userJson = "";
        if (auction.Seller is CorporateCustomer corporateCustomer)
        {
            userJson = JsonSerializer.Serialize<CorporateCustomer>(corporateCustomer);
            sellerType = UserTypes.CorporateCustomer;
        }
        else if (auction.Seller is PrivateCustomer privateCustomer)
        {
            userJson = JsonSerializer.Serialize<PrivateCustomer>(privateCustomer);
            sellerType = UserTypes.PrivateCustomer;
        }
        else
            throw new ArgumentException("unknown user type");
        
        string vehicleJson;
        VehicleTypes vehicleType;
        if (auction.Vehicle is Truck truck)
        {
            vehicleJson = JsonSerializer.Serialize<Truck>(truck);
            vehicleType = VehicleTypes.Truck;
        }
        else if (auction.Vehicle is Bus bus)
        {
            vehicleJson = JsonSerializer.Serialize<Bus>(bus);
            vehicleType = VehicleTypes.Bus;
        }
        else if (auction.Vehicle is ProfessionalPersonalCar professionalPersonalCar)
        {
            vehicleJson = JsonSerializer.Serialize<ProfessionalPersonalCar>(professionalPersonalCar);
            vehicleType = VehicleTypes.ProfessionalPersonalCar;
        }
        else if (auction.Vehicle is PrivatePersonalCar privatePersonalCar)
        {
            vehicleJson = JsonSerializer.Serialize<PrivatePersonalCar>(privatePersonalCar);
            vehicleType = VehicleTypes.PrivatePersonalCar;
        }
        else
        {
            throw new ArgumentException("unknown vehicle type");
        }

        return new AuctionForAPI(auction.Id, vehicleJson, userJson, auction.MinimumPrice, auction.CloseDate,
            vehicleType, sellerType);
    }
}
