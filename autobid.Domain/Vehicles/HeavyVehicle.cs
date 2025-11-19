using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Common;
using autobid.Domain.Common.Enums;                           // License enum

namespace autobid.Domain.Vehicles;

/// <summary>
/// Abstrakt klasse for tunge køretøjer (Bus/Truck). Arver fra Vehicle.
/// </summary>
public abstract class HeavyVehicle : Vehicle
{
    public double HeightMeter { get; set; }                  // Højde i meter
    public double WeightKg { get; set; }                  // Vægt i kg
    public double Length { get; set; }                  // Længde i meter

    protected HeavyVehicle(                             // Ctor passerer fælles felter til base
        uint id, string name, int km, string regNo, int year, double kmPerLiter, Fuel fuel = default)
        : base(id, name, km, regNo, year, kmPerLiter, fuel)               // Kald Vehicle-ctor
    { }

    protected HeavyVehicle()
    {
        
    }

    public override string ToString() =>
       $"{base.ToString()} {nameof(WeightKg)}:{WeightKg} {nameof(HeightMeter)}:{HeightMeter} {nameof(Length)}:{Length}";
}

