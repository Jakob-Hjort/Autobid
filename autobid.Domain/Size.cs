using System.Data.Common;

namespace autobid.Domain;

public record class Size(double W = 0, double H = 0, double L = 0, uint Id = 0)
{
}
