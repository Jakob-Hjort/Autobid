using autobid.Domain.Common;                   // AuctionNotification delegate
using autobid.Domain.Database;
using autobid.Domain.Users;                    // User, CorporateCustomer/PrivateCustomer
using autobid.Domain.Vehicles;                 // Vehicle
using System.Collections.Concurrent;

namespace autobid.Domain.Auctions;

/// <summary>
/// Implementerer auktionshusets forretningsregler:
/// - Opret auktion (overloads)
/// - Modtag bud (validering: højere end nuværende + købers midler/kredit)
/// - Accepter bud (overfør penge, luk auktionen)
/// - Find auktion asynkront (tråd)
/// Notifikationer (delegates) kaldes når min. pris opfyldes og ved accept.
/// </summary>
public sealed class AuctionHouse : IAuctionHouse
{
    private readonly SqlAuctionRepository _repo = new();          // Repo-injektion (Data-lag)
    private readonly AuctionNotification _defaultNotify;// Default notifikation (kan være no-op)

    // Vi kan gemme en specifik notifikation pr. auktion (hvis sat ved oprettelse).
    private readonly ConcurrentDictionary<uint, AuctionNotification> _auctionNotifies
        = new();

    public AuctionHouse(IAuctionRepository repo, AuctionNotification? defaultNotify = null)
    {
        _defaultNotify = defaultNotify ?? ((a, b) => { });// No-op hvis ikke angivet
    }

    // A3 – Overload uden notify: brug default
    public async Task<uint> SetForSale(Vehicle køretøj, User sælger, decimal minimumPris, DateTimeOffset closeDate)
        => await SetForSale(køretøj, sælger, minimumPris, _defaultNotify, closeDate);

    // A3 – Overload med notify: gem notifikation pr. auktion
    public async Task<uint> SetForSale(Vehicle køretøj, User sælger, decimal minimumPris, AuctionNotification notify, DateTimeOffset closeDate)
    {
        var a = new Auction(køretøj, sælger, minimumPris, closeDate);   // Opret domæneobjekt
        var id = await _repo.Add(a);                                // Persistér i DB (eller brug a.Id)
        if (id == 0) id = a.Id;                               // Fallback hvis repo ikke returnerer id
        _auctionNotifies[id] = notify;                        // Gem callback til senere
        return id;                                            // Returnér auktionsnummer
    }

    // A5 – Modtag bud fra køber
    public async Task<bool> TakeBid(User køber, uint auktionsNummer, decimal beløb, AuctionNotification? notify = null)
    {
        var a = await _repo.FindById(auktionsNummer);               // Find auktionen
        if (a is null || a.IsClosed) return false;            // Hvis ikke fundet/allerede lukket → afvis

        var highest = a.HighestBid?.Amount ?? 0m;             // Hent nuværende højeste bud (eller 0)
        if (beløb <= highest) return false;                   // Bud skal være højere end højeste

        // Udregn hvor meget køber må byde for:
        // Privat: må ikke overstige Balance. Erhverv: Balance + Credit.
        decimal maxAllowed = køber switch
        {
            CorporateCustomer c => c.Balance + c.Credit,      // Erhverv: kredit tæller med
            _ => køber.Balance                                // Privat: kun balance
        };
        if (beløb > maxAllowed) return false;                 // Afvis hvis bud overstiger tilladte midler

        var bid = new Bid(køber, beløb);                      // Opret bud-objekt
        a.AddBid(bid);                                        // Læg bud på auktionen (validerer lukket)

        await _repo.AddBid(a.Id, bid);                              // Persistér bud i DB

        // Hvis buddet rammer/overstiger mindsteprisen → send notifikation
        var cb = notify ??                                    // Brug specifik notify hvis angivet
                 (_auctionNotifies.TryGetValue(a.Id, out var n) ? n : _defaultNotify);
        if (beløb >= a.MinimumPrice) cb(a, bid);              // Kald callback

        return true;                                          // Bud accepteret
    }

    // A6 – Sælger accepterer højeste bud
    public async Task<bool> AcceptBid(User sælger, uint auktionsNummer)
    {
        var a = await _repo.FindById(auktionsNummer);               // Find auktionen
        if (a is null || a.IsClosed) return false;            // Afvis hvis ikke fundet/allerede lukket
        if (a.Seller != sælger) return false;                 // Kun sælgeren må acceptere

        var win = a.HighestBid;                               // Hent vinderbud
        if (win is null) return false;                        // Kan ikke acceptere uden bud

        // Tjek at køber stadig kan betale:
        decimal maxAllowed = win.Buyer is CorporateCustomer c ? c.Balance + c.Credit : win.Buyer.Balance;
        if (win.Amount > maxAllowed) return false;            // Hvis ikke længere råd → afvis (frafalder)

        // Overfør penge (simpel kontooverførsel i domain – DB persist sker i Data-lag)
        if (win.Buyer is CorporateCustomer corp)
        {
            // Træk fra balance først; resten implicit dækket af kredit i domænelogik.
            var fromBalance = Math.Min(corp.Balance, win.Amount);
            corp.Balance -= fromBalance;
            // (Hvis vi ville spore faktisk brugt kredit, skulle vi have et felt – krav nævner kun check)
        }
        else
        {
            win.Buyer.Balance -= win.Amount;                  // Privat: træk hele beløbet
        }
        sælger.Balance += win.Amount;                         // Læg beløb til sælgers balance

        a.Close();                                            // Marker auktion som lukket
        _repo.Update(a);                                      // Persistér lukket status

        // Send notifikation om endeligt salg (winning bid)
        var cb = _auctionNotifies.TryGetValue(a.Id, out var n) ? n : _defaultNotify;
        cb(a, win);

        return true;                                          // Accept gennemført
    }

    // A7 – Find auktion asynkront (kører på baggrundstråd via Task.Run)
    public async Task<Auction?> FindAuctionById(uint id)
        => await _repo.FindById(id);      // Deleger til repo inde i Task.Run
}
