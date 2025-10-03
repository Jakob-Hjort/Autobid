using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;                  // Til regex-validering af registreringsnr.
using autobid.Domain.Common;                           // Fuel, License, EnergyClass

namespace autobid.Domain.Vehicles;

/// <summary>
/// Abstrakt basisklasse for ALLE køretøjer. Indeholder fælles felter og valideringer.
/// </summary>
public abstract class Vehicle
{
    public uint Id { get; set; }                      // Primærnøgle fra DB (init: sættes i ctor/object-init)

    private string _name = string.Empty;               // Backing field til Name
    public string Name                                  // Køretøjets navn/beskrivelse
    {
        get => _name;                                   // Returnér feltet
        set => _name = string.IsNullOrWhiteSpace(value) // Tjek for tomt/null
            ? throw new ArgumentException("Navn skal udfyldes", nameof(Name))
            : value.Trim();                             // Trim whitespace i kanter
    }

    private int _distanceTraveledKm;                                   // Backing field for Km
    public int DistanceTraveledKm                                       // Antal kørte kilometer
    {
        get => _distanceTraveledKm;                                     // Returnér
        set => _distanceTraveledKm = value < 0                          // Må ikke være negativt
            ? throw new ArgumentOutOfRangeException(nameof(DistanceTraveledKm))
            : value;
    }

    private string _regNo = string.Empty;              // Backing field for RegistrationNumber
    /// <summary>Reg.nr: 2 bogstaver + 5 cifre (fx AB12345).</summary>
    public string RegistrationNumber
    {
        get => _regNo;                                  // Returnér
        set
        {
            if (value is null)                          // Null-check
                throw new ArgumentNullException(nameof(RegistrationNumber));
            // Regex: præcis 2 bogstaver efterfulgt af 5 cifre
            if (!Regex.IsMatch(value, @"^[A-Za-z]{2}\d{5}$"))
                throw new ArgumentException("Ugyldigt registreringsnr. (forventet: LLDDDDD)");
            _regNo = value.ToUpperInvariant();          // Gem i uppercase
        }
    }

    public int Year { get; init; }                      // Årgang (init—sættes kun i ctor)
    public bool HasTowHitch { get; set; }                  // Træk-krog

    public License LicenseType { get; protected set; } = License.B; // Krævet kørekorttype
    public Fuel Fuel { get; set; }                      // Brændstof
    public double KmPerLiter { get; private set; }              // Brændstoføkonomi (km/l)

    // Motorstørrelse styres via protected setter (subtyper har egne regler for ranges)
    protected void SetEngineLiters(double v, double min, double max)
    {
        if (v < min || v > max)                         // Grænse-check
            throw new ArgumentOutOfRangeException(
                nameof(v), $"Motorstørrelse skal være mellem {min} og {max} L");
        EngineLiters = v;                              // Gem værdi
    }
    public double EngineLiters { get; private set; }        // Offentlig read-only property

    /// <summary>Udregnet energiklasse via helper – opfylder krav V4.</summary>
    public EnergyClass Energy => EnergyClassCalculator.GetFor(this);

    protected Vehicle(                                  // Beskyttet ctor: kun subklasser konstruerer
        uint id, string name, int km, string regNo, int year, double kmPerLiter)
    {
        Id = id;                                        // Sæt Id
        Name = name;                                    // Valideret i set
        DistanceTraveledKm = km;                                        // Valideret i set
        RegistrationNumber = regNo;                     // Valideret i set (regex)
        Year = year;                                    // Ingen validering her (kan tilføjes)
        KmPerLiter = kmPerLiter;
    }

    public override string ToString()                   // Pæn visning i lister/debug
        => $"{nameof(Name)}:{Name} {nameof(Year)}:({Year}) {nameof(RegistrationNumber)}:[{RegistrationNumber}]";
}

