using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Common.Enums;                           // Fuel, License

namespace autobid.Domain.Vehicles;

/// <summary>
/// Bus – regler for motorstørrelse, brændstof og kørekort (krav V5–V9).
/// </summary>
public sealed class Bus : HeavyVehicle                  // sealed: ingen videre arv
{
    public int SeatsAmount { get; set; }                      // Antal siddepladser
    public int BedsAmount { get; set; }                       // Antal sovepladser (campingbus mv.)
    public bool HasToilet { get; set; }                    // Har toilet?

    public Bus(
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch, double kmPerLiter)             // Motorstørrelse + træk-krog
        : base(id, name, km, regNo, year, kmPerLiter)               // Kald HeavyVehicle-ctor
    {
        HasTowHitch = towHitch;                            // Sæt træk
        // Kørekort: D (bus) – med træk bliver det DE (krav V7)
        LicenseType = towHitch ? License.DE : License.D;

        // Motorstørrelse: mellem 4,2L og 15L (krav V8)
        SetEngineLiters(engineLiters, 4.2, 15.0);

        // Brændstof: krav siger diesel (krav V9)
        Fuel = Fuel.Diesel;
    }

    public override string ToString() =>
        $"{base.ToString()}, Amount Of Seats:{SeatsAmount}, Amount Of Beds: {BedsAmount}, Has Toilet: {HasToilet}";
}
