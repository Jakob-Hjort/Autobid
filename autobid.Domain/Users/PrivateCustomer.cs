using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users;                   // Samme namespace som User

/// <summary>
/// Privatkunde – har CPR (krav om separat type).
/// </summary>
public sealed class PrivateCustomer : User         // sealed: kan ikke arves videre
{
    public string CPR { get; }                     // CPR-nummer (string for at bevare leading zeros)

    public PrivateCustomer(                        // Ctor med alle nødvendige felter
        uint id,                                   // Primærnøgle (0 inden DB insert)
        string username,                           // Brugernavn (valideres i base)
        string passwordHash,                       // Hash af password (ikke klartekst)
        string cpr,
        decimal balance)                                // CPR
        : base(id, username, passwordHash, balance)         // Kald base-ctor for fælles felter
    {
        if (string.IsNullOrWhiteSpace(cpr))        // Simpel validering af CPR
            throw new ArgumentException(           // Kaster fejl hvis tom/whitespace
                "CPR mangler", nameof(cpr));
        CPR = cpr;                                 // Sæt property
    }
}

