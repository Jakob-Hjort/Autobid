using autobid.Domain.Auctions;
using autobid.Domain.Services;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.Data.SqlClient;
using System.Data;

namespace autobid.Domain.Database;

// Implementerer dit IAuctionRepository
internal sealed class SqlAuctionRepository : IAuctionRepository
{
    public Auction? FindById(uint id)
    {
        using var conn = Connection.OpenAsync();
        // --- læs auktion + seller + vehicle
        const string sqlA = @"
SELECT a.auctionId, a.minimumPrice, a.isClosed,
       s.userId AS sellerId, s.username, s.passwordHash, s.balance,
       v.vehicleId, v.[name], v.[year]
FROM dbo.auction a
JOIN dbo.[user] s ON s.userId = a.userId
JOIN dbo.vehicle v ON v.vehicleId = a.vehicleId
WHERE a.auctionId = @id;";
        using var cmdA = new SqlCommand(sqlA, conn);
        cmdA.Parameters.Add("@id", SqlDbType.Int).Value = (int)id;

        using var ra = cmdA.ExecuteReader(CommandBehavior.SingleRow);
        if (!ra.Read()) return null;

        var seller = new User(
            id: (uint)ra.GetInt32(3),
            username: ra.GetString(4),
            passwordHash: ra.IsDBNull(5) ? "" : ra.GetString(5),
            balance: ra.GetDecimal(6)
        );
        var vehicle = new Vehicle(
            id: (uint)ra.GetInt32(7),
            name: ra.GetString(8),
            year: (ushort)ra.GetInt32(9)
        );
        var min = ra.GetDecimal(1);
        var closed = ra.GetBoolean(2);
        ra.Close();

        // --- læs bids til denne auktion
        const string sqlB = @"
SELECT b.bidId, b.sendTime, b.amount,
       u.userId, u.username, u.passwordHash, u.balance
FROM dbo.bid b
JOIN dbo.[user] u ON u.userId = b.userId
WHERE b.auctionId = @id
ORDER BY b.sendTime ASC;";
        using var cmdB = new SqlCommand(sqlB, conn);
        cmdB.Parameters.Add("@id", SqlDbType.Int).Value = (int)id;

        var bids = new List<Bid>();
        using var rb = cmdB.ExecuteReader();
        while (rb.Read())
        {
            var bidder = new User(
                id: (uint)rb.GetInt32(3),
                username: rb.GetString(4),
                passwordHash: rb.IsDBNull(5) ? "" : rb.GetString(5),
                balance: rb.GetDecimal(6)
            );
            var bid = new Bid(bidder, rb.GetDecimal(2))
            {
                Time = new DateTimeOffset(rb.GetDateTime(1), TimeSpan.Zero)
            };
            bids.Add(bid);
        }

        return Auction.Rehydrate(id, vehicle, seller, min, closed, bids);
    }

    public IEnumerable<Auction> GetAllOpen()
    {
        using var conn = Connection.OpenAsync();

        // Hent alle åbne auktioner
        const string sqlA = @"
SELECT a.auctionId, a.minimumPrice, a.isClosed,
       s.userId AS sellerId, s.username, s.passwordHash, s.balance,
       v.vehicleId, v.[name], v.[year]
FROM dbo.auction a
JOIN dbo.[user] s ON s.userId = a.userId
JOIN dbo.vehicle v ON v.vehicleId = a.vehicleId
WHERE a.isClosed = 0;";
        using var cmdA = new SqlCommand(sqlA, conn);

        var result = new List<Auction>();
        using var ra = cmdA.ExecuteReader();
        var headers = new List<(uint id, Vehicle v, User s, decimal min)>();
        while (ra.Read())
        {
            var seller = new User(
                id: (uint)ra.GetInt32(3),
                username: ra.GetString(4),
                passwordHash: ra.IsDBNull(5) ? "" : ra.GetString(5),
                balance: ra.GetDecimal(6)
            );
            var vehicle = new Vehicle(
                id: (uint)ra.GetInt32(7),
                name: ra.GetString(8),
                year: (ushort)ra.GetInt32(9)
            );
            headers.Add(((uint)ra.GetInt32(0), vehicle, seller, ra.GetDecimal(1)));
        }
        ra.Close();

        // Hent alle bids til disse auktioner i ét skud
        if (headers.Count == 0) return result;

        // Byg en tabel-parameter (eller lav IN-listen – her vælger vi simpel IN)
        var ids = string.Join(",", headers.Select(h => h.id));
        var sqlB = $@"
SELECT b.auctionId, b.bidId, b.sendTime, b.amount,
       u.userId, u.username, u.passwordHash, u.balance
FROM dbo.bid b
JOIN dbo.[user] u ON u.userId = b.userId
WHERE b.auctionId IN ({ids})
ORDER BY b.auctionId, b.sendTime ASC;";

        var bidsByAuction = new Dictionary<uint, List<Bid>>();
        using (var cmdB = new SqlCommand(sqlB, conn))
        using (var rb = cmdB.ExecuteReader())
        {
            while (rb.Read())
            {
                var aId = (uint)rb.GetInt32(0);
                var bidder = new User(
                    id: (uint)rb.GetInt32(4),
                    username: rb.GetString(5),
                    passwordHash: rb.IsDBNull(6) ? "" : rb.GetString(6),
                    balance: rb.GetDecimal(7)
                );
                var bid = new Bid(bidder, rb.GetDecimal(3))
                {
                    Time = new DateTimeOffset(rb.GetDateTime(2), TimeSpan.Zero)
                };
                if (!bidsByAuction.TryGetValue(aId, out var list))
                    bidsByAuction[aId] = list = new List<Bid>();
                list.Add(bid);
            }
        }

        foreach (var h in headers)
        {
            bidsByAuction.TryGetValue(h.id, out var list);
            result.Add(Auction.Rehydrate(h.id, h.v, h.s, h.min, isClosed: false, bids: list ?? Enumerable.Empty<Bid>()));
        }
        return result;
    }

    public uint Add(Auction auction)
    {
        using var conn = Connection.Open();
        const string sql = @"
INSERT dbo.auction(minimumPrice, isClosed, vehicleId, userId)
OUTPUT INSERTED.auctionId
VALUES (@min, 0, @veh, @usr);";
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@min", SqlDbType.Decimal).Value = auction.MinimumPrice;
        cmd.Parameters.Add("@veh", SqlDbType.Int).Value = (int)auction.Vehicle.Id;
        cmd.Parameters.Add("@usr", SqlDbType.Int).Value = (int)auction.Seller.Id;

        // SQL Server returnerer decimal for OUTPUT identity – cast til int først
        var newId = (int)(decimal)cmd.ExecuteScalar()!;
        return (uint)newId;
    }

    public void Update(Auction auction)
    {
        using var conn = Connection.Open();
        const string sql = @"UPDATE dbo.auction SET isClosed=@c WHERE auctionId=@id;";
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@c", SqlDbType.Bit).Value = auction.IsClosed;
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = (int)auction.Id;
        cmd.ExecuteNonQuery();
    }

    public void AddBid(uint auctionId, Bid bid)
    {
        using var conn = Connection.Open();
        const string sql = @"
INSERT dbo.bid(sendTime, amount, userId, auctionId)
VALUES (@t, @amt, @uid, @aid);";
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@t", SqlDbType.DateTime2).Value = bid.Time.UtcDateTime;
        cmd.Parameters.Add("@amt", SqlDbType.Decimal).Value = bid.Amount;
        cmd.Parameters.Add("@uid", SqlDbType.Int).Value = (int)bid.Buyer.Id;
        cmd.Parameters.Add("@aid", SqlDbType.Int).Value = (int)auctionId;
        cmd.ExecuteNonQuery();
    }
}
