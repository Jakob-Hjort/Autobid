using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Common.Enums
{
    public enum License                               // Enum for kørekorttyper
    {
        A,                                            // MC (ikke brugt her, men komplet)
        B,                                            // Personbil
        BE,                                           // Personbil med tung trailer
        C,                                            // Lastbil
        CE,                                           // Lastbil med tung trailer
        D,                                            // Bus
        DE                                            // Bus med tung trailer
    }
}
