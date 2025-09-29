using System;

namespace autobid.Services;

internal static class Connection
{
    public static string String =>
        Environment.GetEnvironmentVariable("AUTOBID_CS")
        ?? "Server=sql.itcn.dk;Database=jahj23.SKOLE;Trusted_Connection=True;TrustServerCertificate=True";
}
