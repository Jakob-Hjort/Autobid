using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users;                   

/// <summary>
/// Repository-kontrakt som Data-laget (ADO.NET) implementerer.
/// Domain kender kun dette interface – ikke SQL-detaljerne.
/// </summary>
public interface IUserRepository                   // Interface for DB-adgang til brugere
{
    User? FindById(uint id);                       // Hent bruger via Id (nullable hvis ikke fundet)
    User? FindByUsername(string username);         // Hent bruger via brugernavn
    void Add(User user);                           // Indsæt ny bruger i DB
    void Update(User user);                        // Opdater eksisterende bruger i DB
}
