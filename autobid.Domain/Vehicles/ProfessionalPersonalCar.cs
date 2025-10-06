using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using autobid.Domain.Common.Enums;

namespace autobid.Domain.Vehicles;

/// <summary>
/// Erhvervs-personbil – kan kræve BE ved høj trailer-kapacitet (krav V16).
/// </summary>
public sealed class ProfessionalPersonalCar : PersonalCar
{
    public bool HasSafetyBar { get; set; }                 // Sikkerhedsbøjle/afskærmning i varerum
    public int TrailerCapacityKg { get; set; }          // Tilladt trailervægt (kg)

    public ProfessionalPersonalCar(
        uint id, string name, int km, string regNo, int year,
        double engineLiters, bool towHitch, double kmPerLiter, int trailerCapacityKg = 0)
        : base(id, name, km, regNo, year, engineLiters, towHitch, kmPerLiter)
    {
        TrailerCapacityKg = trailerCapacityKg;          // Gem kapacitet

        // Hvis traileren må veje over 750 kg → BE (krav V16)
        if (TrailerCapacityKg > 750)
            LicenseType = License.BE;
    }

    override public string ToString()
        => $"{base.ToString()}, Trailer Capacity: {TrailerCapacityKg}kg, Has SafetyBar: {HasSafetyBar}";
}
