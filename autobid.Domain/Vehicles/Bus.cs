using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Common;                           // Fuel, License

namespace autobid.Domain.Vehicles;

/// <summary>
/// Bus – regler for motorstørrelse, brændstof og kørekort (krav V5–V9).
/// </summary>
public sealed class Bus : HeavyVehicle                  // sealed: ingen videre arv
{
    public int Seats { get; set; }                      // Antal siddepladser
    public int Beds { get; set; }                       // Antal sovepladser (campingbus mv.)
    public bool Toilet { get; set; }                    // Har toilet?

    public Bus(
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch)             // Motorstørrelse + træk-krog
        : base(id, name, km, regNo, year)               // Kald HeavyVehicle-ctor
    {
        TowHitch = towHitch;                            // Sæt træk
        // Kørekort: D (bus) – med træk bliver det DE (krav V7)
        LicenseType = towHitch ? License.DE : License.D;

        // Motorstørrelse: mellem 4,2L og 15L (krav V8)
        SetEngineLiters(engineLiters, 4.2, 15.0);

        // Brændstof: krav siger diesel (krav V9)
        Fuel = Fuel.Diesel;
    }
}
