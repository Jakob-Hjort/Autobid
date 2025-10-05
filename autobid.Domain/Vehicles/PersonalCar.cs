using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Common;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Abstrakt personbil – fælles for Private/Professional (krav V13–V15).
/// </summary>
public abstract class PersonalCar : Vehicle
{
    public int SeatsAmount { get; set; }                      // Antal sæder
    public (double L, double W, double H) Trunk { get; set; } // Bagagerum: længde/bredde/højde i cm/m

    protected PersonalCar(                              // Base-ctor for personbiler
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch, double kmPerLiter)
        : base(id, name, km, regNo, year, kmPerLiter)
    {
        HasTowHitch = towHitch;                            // Træk-krog gemmes
        LicenseType = License.B;                        // Udgangspunkt: B (krav V15)
        // Motorstørrelse: 0,7–10L (krav V13)
        SetEngineLiters(engineLiters, 0.7, 10.0);
        // Brændstof sættes i specifikke subtyper eller ved oprettelse
    }

    public override string ToString() =>
        $"{base.ToString()}, Seats Amount: {SeatsAmount}, Trunk Width: {Trunk.W}, Trunk Height: {Trunk.H}, Trunk Length: {Trunk.L}";
}
