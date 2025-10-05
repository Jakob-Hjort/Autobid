using autobid.Domain.Common;
using autobid.Domain.Services;
using autobid.Domain.Vehicles;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using autobid.Domain.Auctions;

public class BidRepository
{
    public async Task<IEnumerable<BidHistoryEntry>> GetBidHistoryForUserAsync(uint userId)
    {
        const string sql = @"
        SELECT 
            v.[name]      AS VehicleName,
            v.[year]      AS [Year],
            ub.UserMax    AS BidAmount,
            am.AuctionMax AS FinalAmount,
            CASE WHEN ub.UserMax = am.AuctionMax THEN 1 ELSE 0 END AS IsWinner
        FROM (SELECT DISTINCT auctionId FROM bid WHERE userId = @userId) a
        JOIN auction au ON au.auctionId = a.auctionId
        JOIN vehicle v  ON v.vehicleId  = au.vehicleId
        CROSS APPLY (SELECT MAX(amount) AS AuctionMax FROM bid WHERE auctionId = a.auctionId) am
        CROSS APPLY (SELECT MAX(amount) AS UserMax   FROM bid WHERE auctionId = a.auctionId AND userId = @userId) ub
        ORDER BY au.auctionId DESC;";

        await using var conn = await Connection.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        var list = new List<BidHistoryEntry>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new BidHistoryEntry
            {
                VehicleName = reader.GetString(0),
                Year = reader.GetInt16(1),
                BidAmount = reader.GetDecimal(2),
                FinalAmount = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                IsWinner = reader.GetInt32(4) == 1
            });
        }

        return list;
    }
}
