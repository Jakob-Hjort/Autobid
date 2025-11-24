using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Common.Enums;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Abstrakt personbil – fælles for Private/Professional (krav V13–V15).
/// </summary>
public abstract class PersonalCar : Vehicle
{
    const string TypeName = "PersonalCar";
    public int SeatsAmount { get; set; }                      // Antal sæder
    public Size Trunk { get; set; } = new();

    protected PersonalCar(                              // Base-ctor for personbiler
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch, double kmPerLiter, Fuel fuel = default)
        : base(id, name, km, regNo, year, kmPerLiter, fuel)
    {
        HasTowHitch = towHitch;                            // Træk-krog gemmes
        LicenseType = License.B;                        // Udgangspunkt: B (krav V15)
        // Motorstørrelse: 0,7–10L (krav V13)
        SetEngineLiters(engineLiters, 0.7, 10.0);
        // Brændstof sættes i specifikke subtyper eller ved oprettelse
    }

    public PersonalCar()
    {
    }

    public override string ToString() =>
        $"{base.ToString()}, Seats Amount: {SeatsAmount}, Trunk Width: {Trunk.W}, Trunk Height: {Trunk.H}, Trunk Length: {Trunk.L}";
}
