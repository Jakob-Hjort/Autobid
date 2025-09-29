using System;
using Microsoft.Data.SqlClient;


namespace autobid.Domain.Services;

internal static class Connection
{
    public static string String =>
        Environment.GetEnvironmentVariable("AUTOBID_CS")
        ?? "Server=sql.itcn.dk;Database=jahj23.SKOLE; User ID=jahj2.SKOLE;Password=SJW56pm88b;Trusted_Connection=True;TrustServerCertificate=True";


    public static async Task<SqlConnection> OpenAsync()
    {
        var conn = new SqlConnection(String);
        await conn.OpenAsync();
        return conn;
    }
}


