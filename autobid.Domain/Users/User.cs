using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users;                   // Namespace for brugertyper

/// <summary>
/// Basisklasse for bruger: fælles felter, simpel validering og ToString().
/// Arves af PrivateCustomer og CorporateCustomer.
/// </summary>
public abstract class User                         // Abstrakt: man laver ikke "User" direkte
{
    public uint Id { get; set; }                   // Primærnøgle (fra DB Identity/Sequence)

    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 16;

    private string _username = string.Empty;       // Privat backing-field til Username
    public string Username                          // Offentlig property for brugernavn
    {
        get => _username;                           // Getter: returnér feltet
        init                                         // init: kan sættes i ctor/object-init, men ikke ændres bagefter
        {
            if (string.IsNullOrWhiteSpace(value))    // Valider: må ikke være tomt/null
                throw new ArgumentException(         // Kaster fejl ved ugyldig værdi
                    "Brugernavn skal udfyldes", nameof(Username));
            _username = value.Trim();               // Trim: fjern mellemrum i kanterne
        }
    }

    public string PasswordHash { get; private set; } // Hash af password (aldrig klartekst)

    public decimal Balance { get; set; }            // Konto/balance – bruges når man byder/køber

    protected User(uint id, string username, string passwordHash) // Beskyttet ctor: kun subklasser må konstruere
    {
        Id = id;                                    // Sæt Id (0 hvis ikke fra DB endnu)
        Username = username;                        // Sæt brugernavn (rammer init-setter med validering)
        PasswordHash = passwordHash;                // Sæt hash (kommer typisk fra DB eller via SetPassword)
    }

    public void SetPassword(string raw, IPasswordHasher hasher)   // Metode til at opdatere hash
        => PasswordHash = hasher.Hash(raw);        // Brug injiceret hasher til at generere hash

    public override string ToString()               // Override af ToString for pæn debug/visning
        => $"{Username} ({GetType().Name})";        // F.eks. "jakob (PrivateCustomer)"
}

