using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Common;                           // License enum

namespace autobid.Domain.Vehicles;

/// <summary>
/// Abstrakt klasse for tunge køretøjer (Bus/Truck). Arver fra Vehicle.
/// </summary>
public abstract class HeavyVehicle : Vehicle
{
    public double Height { get; set; }                  // Højde i meter
    public double Weight { get; set; }                  // Vægt i kg
    public double Length { get; set; }                  // Længde i meter

    protected HeavyVehicle(                             // Ctor passerer fælles felter til base
        uint id, string name, int km, string regNo, int year)
        : base(id, name, km, regNo, year)               // Kald Vehicle-ctor
    { }
}

