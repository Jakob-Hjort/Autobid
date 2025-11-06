using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users
{
    public sealed class CorporateCustomer : User       // sealed: ingen yderligere arvinger
    {
        public string CVR { get; set; }                    // CVR-nummer (string)
        public decimal Credit { get; set; }            // Kredit – kan lægges oven i Balance ved bud
        public CorporateCustomer(                      // Ctor
            uint id = 0,                                   // Id (DB)
            string username = "",                           // Brugernavn
            string passwordHash = "",                       // Hash
            string cvr = "",                                // CVR
            decimal credit = 0,                             // Kredit
            decimal balance = 0)                            // Kredit
            : base(id, username, passwordHash, balance)         // Base-fællesfelter
        {
            if (string.IsNullOrWhiteSpace(cvr))        // Valider CVR
                throw new ArgumentException(           // Kaster fejl hvis ugyldig
                    "CVR mangler", nameof(cvr));
            CVR = cvr;                                 // Sæt CVR
            Credit = credit;                           // Sæt Kredit
        }

        public CorporateCustomer()
        {
            
        }
    }
}
