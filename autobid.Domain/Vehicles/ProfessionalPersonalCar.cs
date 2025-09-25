using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Common;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Erhvervs-personbil – kan kræve BE ved høj trailer-kapacitet (krav V16).
/// </summary>
public sealed class ProfessionalPersonalCar : PersonalCar
{
    public bool SafetyBar { get; set; }                 // Sikkerhedsbøjle/afskærmning i varerum
    public int TrailerCapacityKg { get; set; }          // Tilladt trailervægt (kg)

    public ProfessionalPersonalCar(
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch, int trailerCapacityKg = 0)
        : base(id, name, km, regNo, year, engineLiters, towHitch)
    {
        TrailerCapacityKg = trailerCapacityKg;          // Gem kapacitet

        // Hvis traileren må veje over 750 kg → BE (krav V16)
        if (TrailerCapacityKg > 750)
            LicenseType = License.BE;
    }
}
