using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Common
{
    /// <summary>Brændstoftype – bruges i energiklasse-regler.</summary>
    public enum Fuel                                  // En enum er et sæt navngivne værdier
    {                                                 // Start på enum-blok
        Diesel,                                       // Diesel-brændstof
        Benzin,                                       // Benzin-brændstof
        Elektrisk,                                    // El (batteri)
        Hydrogen                                      // Brint
    }                                                 // Slut på enum

    /// <summary>Kørekorttyper – bruges af køretøjstyper til regler.</summary>
    public enum License                               // Enum for kørekorttyper
    {
        A,                                            // MC (ikke brugt her, men komplet)
        B,                                            // Personbil
        BE,                                           // Personbil med tung trailer
        C,                                            // Lastbil
        CE,                                           // Lastbil med tung trailer
        D,                                            // Bus
        DE                                            // Bus med tung trailer
    }

    /// <summary>Energiklasse – udregnes ud fra km/l + årgang + brændstof.</summary>
    public enum EnergyClass                           // Enum for energiklasse A–D
    {
        A,                                            // Bedst
        B,                                            // God
        C,                                            // Middel
        D                                             // Dårligst
    }
}
