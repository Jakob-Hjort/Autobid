using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Users;                     // Vi skal kende typen User (køberen)
namespace autobid.Domain.Auctions;              // Namespace for auktioner og bud

/// <summary>
/// Et enkelt bud på en auktion.
/// </summary>
public sealed class Bid                         // sealed: kan ikke arves (simpel value-objekt)
{
    public DateTimeOffset Time { get; init; }   // Hvornår buddet blev afgivet
    public User Buyer { get; init; }            // Hvem der bød (brugerobjekt)
    public decimal Amount { get; init; }        // Budbeløb

    public Bid(User buyer, decimal amount)      // Konstruktør med de nødvendige felter
    {
        Buyer = buyer;                          // Sæt køberen
        Amount = amount;                        // Sæt beløbet
        Time = DateTimeOffset.Now;              // Timestamp sættes automatisk
    }

    public override string ToString()           // Pæn visning i log/debug
        => $"{Buyer.Username} bød {Amount:n2} kl. {Time:HH:mm:ss}";
}

