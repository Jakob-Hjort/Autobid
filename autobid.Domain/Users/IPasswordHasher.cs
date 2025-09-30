using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users;                   

/// <summary>
/// Abstraktion for hashing af passwords – så Domain
/// er uafhængig af konkret algoritme (DI-venligt).
/// </summary>
public interface IPasswordHasher                   // Interface-definition
{
    string Hash(string raw);                       // Metode: returnér hash fra klartekst
    bool Verify(string raw, string hash);          // Metode: verificér at klartekst matcher hash
}
