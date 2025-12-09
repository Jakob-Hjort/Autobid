using System.Runtime.CompilerServices;
using System.Text.Json;
using autobid.Domain.Auctions;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;

namespace autobid.Domain.API;

public record BidForApi(string UserJson, UserTypes UserType, AuctionForAPI AuctionForAPI, decimal MinimumPrice, uint Id = 0)
{
    public Bid ToBid()
    {
        User? user = UserType switch
        {
            UserTypes.CorporateCustomer => JsonSerializer.Deserialize<CorporateCustomer>(UserJson),
            UserTypes.PrivateCustomer=> JsonSerializer.Deserialize<PrivateCustomer>(UserJson),
            _ => throw new InvalidDataException()
        };

        if (user == null)
        {
            throw new InvalidDataException();
        }

        Auction auction = AuctionForAPI.ToAuction();
        
        return new(user, MinimumPrice, Id) {Auction = auction};
    }

    public static BidForApi FromBid(Bid bid)
    {
        AuctionForAPI auctionForAPI = AuctionForAPI.FromAuction(bid.Auction); 
        UserTypes userType = bid.Buyer switch
        {
            CorporateCustomer => UserTypes.CorporateCustomer,
            PrivateCustomer => UserTypes.PrivateCustomer, 
             _ => throw new InvalidDataException()
        };
        string userJson = JsonSerializer.Serialize(bid.Buyer);

        return new BidForApi(userJson, userType, auctionForAPI, bid.Amount, bid.Id);
    }
}
