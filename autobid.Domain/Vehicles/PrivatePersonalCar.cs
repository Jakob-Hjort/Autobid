using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Privat personbil (krav V17–V19).
/// </summary>
public sealed class PrivatePersonalCar : PersonalCar
{
    public bool HasIsofix { get; set; }                    // Har bilen ISOFIX-beslag?

    public PrivatePersonalCar(
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch, double kmPerLiter)
        : base(id, name, km, regNo, year, engineLiters, towHitch, kmPerLiter)
    {
        // Intet særligt ud over basis-reglerne i PersonalCar
    }
}
