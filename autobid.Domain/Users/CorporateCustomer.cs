using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users;                   // Samme namespace

/// <summary>
/// Erhvervskunde – CVR og Credit (kreditgrænse for bud).
/// </summary>
public sealed class CorporateCustomer : User       // sealed: ingen yderligere arvinger
{
    public string CVR { get; }                     // CVR-nummer (string)
    public decimal Credit { get; set; }            // Kredit – kan lægges oven i Balance ved bud

    public CorporateCustomer(                      // Ctor
        uint id,                                   // Id (DB)
        string username,                           // Brugernavn
        string passwordHash,                       // Hash
        string cvr,                                // CVR
        decimal credit,
        decimal balance)                            // Kredit
        : base(id, username, passwordHash, balance)         // Base-fællesfelter
    {
        if (string.IsNullOrWhiteSpace(cvr))        // Valider CVR
            throw new ArgumentException(           // Kaster fejl hvis ugyldig
                "CVR mangler", nameof(cvr));
        CVR = cvr;                                 // Sæt CVR
        Credit = credit;                           // Sæt Kredit
    }
}

