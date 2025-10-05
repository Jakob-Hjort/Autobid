using autobid.Domain.Auctions;
using autobid.Domain.Services;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace autobid.Domain.Database;

public sealed class SqlAuctionRepository : IAuctionRepository
{
    CarRepository _carRepository = new();
    UserRepository _userRepository = new();

    public async Task<uint> Add(Auction auction)
    {
        auction.Vehicle.Id = Convert.ToUInt32(await _carRepository.Add(auction.Vehicle));

        string sql = @"
        INSERT INTO auction(userId, minimumPrice, isClosed, vehicleId, closeDate)
        VALUES(@userId, @minimumPrice, @isClosed, @vehicleId, @closeDate);
        SELECT @NewAuctionId = CAST(SCOPE_IDENTITY() AS INT);
    ";
        using SqlConnection conn = await Connection.OpenAsync();
        using SqlCommand cmd = new(sql, conn);

        cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = Convert.ToInt32(auction.Seller.Id) });
        var minimumPriceParam = new SqlParameter("@minimumPrice", SqlDbType.Decimal) { Value = auction.MinimumPrice };
        minimumPriceParam.Precision = 18;
        minimumPriceParam.Scale = 2;
        cmd.Parameters.Add(minimumPriceParam);
        cmd.Parameters.Add(new SqlParameter("@closeDate", SqlDbType.DateTime) { Value = auction.CloseDate.DateTime });
        cmd.Parameters.Add(new SqlParameter("@isClosed", SqlDbType.Bit) { Value = Convert.ToInt32(auction.IsClosed) });
        cmd.Parameters.Add(new SqlParameter("@vehicleId", SqlDbType.Int) { Value = Convert.ToInt32(auction.Vehicle.Id) });

        var pOut = new SqlParameter("@NewAuctionId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(pOut);

        await cmd.ExecuteNonQueryAsync();

        return Convert.ToUInt32(pOut.Value);
    }


    public async Task AddBid(uint auctionId, Bid bid)
    {
        string sql = @"
			INSERT INTO bid(auctionId, userId, amount, sendTime)
			VALUES(@auctionId, @userId, @amount, @sendTime)
		";
        using SqlConnection conn = await Connection.OpenAsync();
        using SqlCommand cmd = new(sql, conn);

        cmd.Parameters.Add(new("@amount", SqlDbType.Decimal) { Value = bid.Amount });
        cmd.Parameters.AddWithValue("@userId", (int)bid.Buyer.Id);
        cmd.Parameters.AddWithValue("@auctionId", (int)auctionId);
        cmd.Parameters.AddWithValue("@sendTime", bid.Time.DateTime);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Auction?> FindById(uint id)
    {
        string sql = @"SELECT * FROM auction WHERE auctionId = @auctionId";
        await using SqlConnection conn = await Connection.OpenAsync();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@auctionId", Convert.ToInt32(id));
        SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (reader.Read())
        {
            int vehicleId = reader.GetInt32(reader.GetOrdinal("vehicleId"));
            uint userId = Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("userId")));
            Vehicle? vehicle = await _carRepository.GetSingle(vehicleId);
            User? user = await _userRepository.FindById(userId);
            if (vehicle == null || user == null)
                return null;

            Auction auction = new(
                vehicle,
                user,
                reader.GetDecimal(reader.GetOrdinal("minimumPrice")),
                reader.GetDateTime(reader.GetOrdinal("closeDate")),
                Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("auctionId")))
            );

            foreach (Bid bid in await GetAllBidsForAuction(auction))
            {
                auction.AddBid(bid);
            }

            return auction;
        }

        return null;
    }

    async Task<IEnumerable<Bid>> GetAllBidsForAuction(Auction auction)
    {
        string sql = @"
			SELECT b.*, u.username FROM bid AS b
			INNER JOIN [user] AS u ON u.userId = b.userId
			WHERE b.auctionId = @auctionId
			ORDER BY b.sendTime DESC
		";
        await using SqlConnection conn = await Connection.OpenAsync();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@auctionId", Convert.ToInt32(auction.Id));
        var reader = await cmd.ExecuteReaderAsync();
        var bids = new List<Bid>();
        while (reader.Read())
        {
            uint userId = Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("userId")));
            User? user = await _userRepository.FindById(userId);
            if (user == null)
                continue;
            bids.Add(new Bid(
                user,
                reader.GetDecimal(reader.GetOrdinal("amount"))
            )
            {
                Time = reader.GetDateTime(reader.GetOrdinal("sendTime"))
            });
        }
        return bids;
    }

    public async Task<IEnumerable<AuctionListItem>> GetAllAuctonOpenListItems()
    {
        string sql = @"
        SELECT v.[name],
		au.auctionId,
		(SELECT TOP(1) MAX(amount) FROM bid
		WHERE auctionId = au.auctionId) as highestBid,
		v.[year] ,u.username FROM auction as au
		INNER JOIN [user] AS u ON u.userId = au.userId
		INNER JOIN vehicle AS v ON v.vehicleId = au.vehicleId
		WHERE GETDATE() < closeDate AND au.isClosed = 0
		";

        await using SqlConnection conn = await Connection.OpenAsync();

        await using SqlCommand cmd = new(sql, conn);

        var reader = await cmd.ExecuteReaderAsync();
        var items = new List<AuctionListItem>();
        while (reader.Read())
        {
            int highestBidOrdinal = reader.GetOrdinal("highestBid");


            items.Add(
                new(
                    Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("auctionId"))),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetInt16(reader.GetOrdinal("year")),
                    await reader.IsDBNullAsync(highestBidOrdinal) ? 0 : reader.GetDecimal(highestBidOrdinal),
                    reader.GetString(reader.GetOrdinal("username"))
                )
            );
        }

        return items;
    }

    public async Task CloseAuction(uint auctionId)
    {
        string sql = @"
			UPDATE auction SET isClosed = 1
			WHERE auctionId = @auctionId
		";
        await using SqlConnection conn = await Connection.OpenAsync();

        await using SqlCommand cmd = new(sql, conn);

        cmd.Parameters.AddWithValue("@auctionId", (int)auctionId);
        await cmd.ExecuteNonQueryAsync();
    }

    public IEnumerable<Auction> GetAllOpen()
    {
        throw new NotImplementedException();
    }

    public void Update(Auction auction)
    {
        throw new NotImplementedException();
    }
}
