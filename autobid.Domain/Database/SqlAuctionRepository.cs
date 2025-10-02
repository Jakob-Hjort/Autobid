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
		string sql = @"
			INSERT INTO auction(userId, minimumPrice, isClosed, vehicleId)
			VALUES(@userId, @minimumPrice, @isClosed, @vehicleId);
			SET @NewAuctionId = SCOPE_IDENTITY();
		";
		using SqlConnection conn = await Connection.OpenAsync();
		using SqlCommand cmd = new(sql, conn);

		cmd.Parameters.AddWithValue("@userId", auction.Seller.Id);
		cmd.Parameters.AddWithValue("@minimumPrice", auction.MinimumPrice);
		cmd.Parameters.AddWithValue("@isClosed", auction.IsClosed);
		cmd.Parameters.AddWithValue("@vehicleId", auction.Vehicle.Id);
		var pOut = new SqlParameter("@NewAuctionId", SqlDbType.Int) { Direction = ParameterDirection.Output };

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

		cmd.Parameters.AddWithValue("@amount", bid.Amount);
		cmd.Parameters.AddWithValue("@userId", bid.Buyer.Id);
		cmd.Parameters.AddWithValue("@auctionId", auctionId);
		cmd.Parameters.AddWithValue("@sendTime", bid.Time);
		await cmd.ExecuteNonQueryAsync();

	}

	public async Task<Auction?> FindById(uint id)
	{
		string sql = @"";
		await using SqlConnection conn = await Connection.OpenAsync();
		await using SqlCommand cmd = new(sql, conn);

		SqlDataReader reader = await cmd.ExecuteReaderAsync();

		if (reader.Read())
		{
			int vehicleId = reader.GetInt32(reader.GetOrdinal("vehicleId"));
			uint userId = Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("userId")));
			Vehicle? vehicle = await _carRepository.GetSingle(vehicleId);
			User? user = await _userRepository.FindById( userId);
			if (vehicle == null || user == null) 
				return null;

			return new(
				vehicle,
				user,
				reader.GetDecimal(reader.GetOrdinal("minimumPrice")),
				Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("auctionId")))
				);
		}

		return null;
	}

	public async Task<IEnumerable<AuctionListItem>> GetAllAuctonOpenListItems()
    {
        string sql = @"
        SELECT v.[name],
		au.auctionId,
        ROW_NUMBER() OVER (PARTITION BY v.[name] ORDER BY Amount DESC, au.auctionId DESC) AS bid,
        v.[year] ,u.username FROM auction as au
        INNER JOIN [user] AS u ON u.userId = au.userId
        INNER JOIN vehicle AS v ON v.vehicleId = au.vehicleId
        INNER JOIN bid AS b ON b.auctionId = au.auctionId
		WHERE au.isClosed = 0
        ORDER BY b.amount DESC
		";

        await using SqlConnection conn = await Connection.OpenAsync();

        await using SqlCommand cmd = new(sql, conn);

        var reader = await cmd.ExecuteReaderAsync();
        var items = new List<AuctionListItem>();
        while (reader.Read())
        {
            items.Add(
                new(
					Convert.ToUInt32(reader.GetInt32(reader.GetOrdinal("auctionId"))),
					reader.GetString(reader.GetOrdinal("name")),
					reader.GetInt32(reader.GetOrdinal("year")),
					reader.GetDecimal(reader.GetOrdinal("bid")),
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

		cmd.Parameters.AddWithValue("@auctionId", auctionId);
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

	Auction? IAuctionRepository.FindById(uint id)
	{
		throw new NotImplementedException();
	}

	uint IAuctionRepository.Add(Auction auction)
	{
		throw new NotImplementedException();
	}

	void IAuctionRepository.AddBid(uint auctionId, Bid bid)
	{
		throw new NotImplementedException();
	}
}
