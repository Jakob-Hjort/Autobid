namespace autobid.Domain.API;

public record class AuctionForAPI(uint Id, string VehicleJson, string SellerJson, decimal MinPrice, DateTimeOffset CloseDate, string VehicleType, string SellerType);
