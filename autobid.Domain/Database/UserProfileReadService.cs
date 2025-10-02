using autobid.Domain.Users;
using autobid.Domain.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace autobid.Domain.Database;

public sealed class UserProfileReadService : IUserProfileReadService
{
    public async Task<UserProfileSummary> GetAsync(uint userId)
    {
        await using var conn = await Connection.OpenAsync();

        // 1) Hent basis fra [user]
        const string sqlUser = @"SELECT username, balance FROM dbo.[user] WHERE userId = @uid;";
        await using var cmdUser = new SqlCommand(sqlUser, conn);
        cmdUser.Parameters.Add("@uid", SqlDbType.Int).Value = (int)userId;

        using var r = await cmdUser.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await r.ReadAsync())
            throw new InvalidOperationException("User not found.");
        var username = r.GetString(0);
        var balance = r.GetDecimal(1);
        r.Close();

        // 2) Counts: egne auktioner + vundne auktioner
        // - Sælger: auction.userId = @uid
        // - Vundet: lukket auktion hvor top-bud (højeste amount; ved tie tidligste sendTime) er fra @uid
        const string sqlCounts = @"
        SELECT 
          SellerCount = (SELECT COUNT(*) FROM dbo.auction a WHERE a.userId = @uid),
          WonCount    = (
            SELECT COUNT(*)
            FROM dbo.auction a
            CROSS APPLY (
                SELECT TOP (1) b.userId, b.amount, b.sendTime
                FROM dbo.bid b
                WHERE b.auctionId = a.auctionId
                ORDER BY b.amount DESC, b.sendTime ASC
            ) topBid
            WHERE a.closeDate = GETDATE() OR a.isClosed = 1
              AND topBid.userId = @uid
              -- Hvis I kun vil tælle vundne når minimumsprisen er nået, så fjern kommentaren herunder:
              -- AND topBid.amount >= a.minimumPrice
          );";

        await using var cmdCounts = new SqlCommand(sqlCounts, conn);
        cmdCounts.Parameters.Add("@uid", SqlDbType.Int).Value = (int)userId;

        using var rc = await cmdCounts.ExecuteReaderAsync(CommandBehavior.SingleRow);
        await rc.ReadAsync();
        var yourAuctions = rc.IsDBNull(0) ? 0 : rc.GetInt32(0);
        var won = rc.IsDBNull(1) ? 0 : rc.GetInt32(1);

        return new UserProfileSummary(userId, username, balance, yourAuctions, won);
    }
}
