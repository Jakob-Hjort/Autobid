using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Common;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Lastbil – lignende regler som bus (krav V10–V12).
/// </summary>
public sealed class Truck : HeavyVehicle
{
    public int PayloadKg { get; set; }                  // Nyttelast i kg

    public Truck(
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch)
        : base(id, name, km, regNo, year)
    {
        HasTowHitch = towHitch;
        // Kørekort: C – med træk CE (krav V10)
        LicenseType = towHitch ? License.CE : License.C;

        // Motorstørrelse: 4,2–15L (krav V11)
        SetEngineLiters(engineLiters, 4.2, 15.0);

        // Brændstof: diesel (krav V12)
        Fuel = Fuel.Diesel;
    }
}
