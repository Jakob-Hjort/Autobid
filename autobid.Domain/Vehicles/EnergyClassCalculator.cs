using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Common;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Statisk hjælper der beregner energiklasse ud fra reglerne (krav V4).
/// </summary>
public static class EnergyClassCalculator
{
    public static EnergyClass GetFor(Vehicle v)         // Offentlig metode: tag et Vehicle og returér klasse
    {
        // El og brint: altid A (krav)
        if (v.Fuel is Fuel.Elektrisk or Fuel.Hydrogen)
            return EnergyClass.A;

        bool post2010 = v.Year >= 2010;                 // Skelnen før/efter 2010
        // Diesel-regler
        if (v.Fuel == Fuel.Diesel)
        {
            if (post2010)
                return v.KmPerLiter switch               // Pattern matching på km/l
                {
                    >= 25 => EnergyClass.A,
                    >= 20 => EnergyClass.B,
                    >= 15 => EnergyClass.C,
                    _ => EnergyClass.D
                };
            // Før 2010
            return v.KmPerLiter switch
            {
                >= 23 => EnergyClass.A,
                >= 18 => EnergyClass.B,
                >= 13 => EnergyClass.C,
                _ => EnergyClass.D
            };
        }

        // Benzin-regler
        if (post2010)
            return v.KmPerLiter switch
            {
                >= 20 => EnergyClass.A,
                >= 16 => EnergyClass.B,
                >= 12 => EnergyClass.C,
                _ => EnergyClass.D
            };

        // Benzin før 2010
        return v.KmPerLiter switch
        {
            >= 18 => EnergyClass.A,
            >= 14 => EnergyClass.B,
            >= 10 => EnergyClass.C,
            _ => EnergyClass.D
        };
    }
}

