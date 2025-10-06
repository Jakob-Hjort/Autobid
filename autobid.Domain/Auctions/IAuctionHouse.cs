using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Common;                   // AuctionNotification delegate
using autobid.Domain.Users;                    // User-typer
using autobid.Domain.Vehicles;                 // Vehicle-typer

namespace autobid.Domain.Auctions;

/// <summary>
/// Forretningslogik for auktionshuset (use cases fra kravene).
/// </summary>
public interface IAuctionHouse
{
    Task<uint> SetForSale(Vehicle vehicle, User seller, decimal minimumPrice, DateTimeOffset closeDate);

    // A6: Sælger accepterer højeste bud (overfører penge og lukker auktion)
    Task<bool> AcceptBid(User sælger, uint auktionsNummer);

    // A7: Find auktion med id – trådet (Task.Run)
    Task<Auction?> FindAuctionById(uint id);
}

