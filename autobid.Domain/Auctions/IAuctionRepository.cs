using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Auctions;              // Repo-kontrakt – implementeres i Data-laget

/// <summary>
/// Repository for auktioner. Domain kender kun interfacet –
/// Data (ADO.NET) leverer implementeringen mod SQL.
/// </summary>
public interface IAuctionRepository
{
    Auction? FindById(uint id);                 // Hent én auktion (kan være null)
    IEnumerable<Auction> GetAllOpen();          // Hent alle åbne auktioner
    uint Add(Auction auction);                  // Opret og returér id (kan ignoreres ved static id)
    void Update(Auction auction);               // Gem ændringer (fx lukket status)
    void AddBid(uint auctionId, Bid bid);       // Tilføj bud på en auktion
    
}

