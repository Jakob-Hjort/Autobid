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
    Task<Auction?> FindById(uint id);                 // Hent én auktion (kan være null)
    Task<uint> Add(Auction auction);                  // Opret og returér id (kan ignoreres ved static id)
    Task AddBid(uint auctionId, Bid bid);             // Tilføj bud på en auktion
    
}

