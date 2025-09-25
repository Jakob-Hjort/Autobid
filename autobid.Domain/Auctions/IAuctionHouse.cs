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
    // A3: Sæt køretøj til salg (overload uden/med notifikations-delegate)
    uint SætTilSalg(Vehicle køretøj, User sælger, decimal minimumPris);
    uint SætTilSalg(Vehicle køretøj, User sælger, decimal minimumPris, AuctionNotification notify);

    // A5: Modtag bud (evt. med specifik notifikation)
    bool ModtagBud(User køber, uint auktionsNummer, decimal beløb, AuctionNotification? notify = null);

    // A6: Sælger accepterer højeste bud (overfører penge og lukker auktion)
    bool AccepterBud(User sælger, uint auktionsNummer);

    // A7: Find auktion med id – trådet (Task.Run)
    Task<Auction?> FindAuktionMedIDAsync(uint id, CancellationToken ct = default);
}

