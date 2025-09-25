using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Vehicles;                     // Namespace for køretøjs-relaterede typer

/// <summary>
/// Repository-kontrakt for køretøjer (implementeres i Data-laget).
/// </summary>
public interface IVehicleRepository
{
    Vehicle? FindById(uint id);                        // Hent ét køretøj
    IEnumerable<Vehicle> GetAll();                     // Hent alle (evt. paging senere)
    uint Add(Vehicle v);                               // Opret og returnér db-id
    void Update(Vehicle v);                            // Opdater felter
}

