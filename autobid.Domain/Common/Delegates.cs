using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Auctions;                 // Vi refererer nu de to typer i samme assembly

namespace autobid.Domain.Common;

/// <summary>
/// Delegate til notifikationer fra auktionshuset (krav: delegates/callbacks).
/// Kaldes fx når mindstepris nås eller når en auktion accepteres.
/// </summary>
public delegate void AuctionNotification(Auction auction, Bid? winningBid);

