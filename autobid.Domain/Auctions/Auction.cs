using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Users;                     // Sælger er en User
using autobid.Domain.Vehicles;                  // Den vare der sælges er et Vehicle

namespace autobid.Domain.Auctions;

/// <summary>
/// En auktion over et enkelt køretøj.
/// Indeholder min. pris, alle bud og status (åben/lukket).
/// </summary>
/// 
public sealed class Auction
{
    private static uint _nextId = 1;            // Statisk tæller (krav: static member/metode)
    public static uint NextId() => _nextId++;   // Statisk metode: generér nyt id
    public DateTimeOffset CloseDate { get; set; }
    public uint AuctionId { get; set; } = 0;
    public uint Id { get; }                     // Auktionsnummer
    public Vehicle Vehicle { get; }             // Køretøjet der sælges
    public User Seller { get; }                 // Sælgeren
    public decimal MinimumPrice { get; }        // Mindstepris (når opfyldt → notifikation)
    public bool IsClosed { get; private set; }  // Lukket efter accept

    private readonly List<Bid> _bids = new();   // Intern liste af bud
    public IReadOnlyList<Bid> Bids => _bids;    // Offentlig read-only visning

    public Bid? HighestBid                      // Hjælper: hent højeste bud (eller null)
        => _bids.OrderByDescending(b => b.Amount).FirstOrDefault();

    public Auction(Vehicle vehicle, User seller, decimal minPrice,DateTimeOffset closeDate, uint auctionId = 0) // Ctor
	{
		Id = NextId();                          // Tildel nyt id fra statisk tæller
		Vehicle = vehicle;                      // Gem reference til bilen
		Seller = seller;                        // Gem sælger
		MinimumPrice = minPrice;                // Gem mindstepris
		AuctionId=auctionId;
        CloseDate = closeDate;
    }

	 public void AddBid(Bid bid)                 // Intern helper: læg bud på listen
    {
        if (IsClosed)                           // Ingen bud på lukket auktion
            throw new InvalidOperationException("Auktionen er lukket.");
        _bids.Add(bid);                         // Tilføj bud
    }

    public void Close() => IsClosed = true;     // Marker auktionen som lukket (ved accept)

    public override string ToString()           // Pæn tekst
        => $"#{Id} – {Vehicle.Name} – Min: {MinimumPrice:n0} – Bud: {_bids.Count}";
}
