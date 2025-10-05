using autobid.Domain.Security;
using autobid.Domain.Services;
using autobid.Domain.Users;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Database
{
    public class UserRepository
    {
        public async Task<int> Add(PrivateCustomer user)
        {
			await using var conn = await Connection.OpenAsync(); // returns SqlConnection
			await using var cmd = new SqlCommand("dbo.AppCreatePrivateUser", conn)
			{
				CommandType = CommandType.StoredProcedure
			};

			cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 32) { Value = (object?)user.Username ?? DBNull.Value });
			cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = (object?)user.PasswordHash ?? DBNull.Value });
			cmd.Parameters.Add(new SqlParameter("@CPR", SqlDbType.VarChar, 16) { Value = (object?)user.CPR ?? DBNull.Value });
			cmd.Parameters.Add(new SqlParameter("@InitialBalance", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = user.Balance });

			var pOut = new SqlParameter("@NewUserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
			cmd.Parameters.Add(pOut);

			await cmd.ExecuteNonQueryAsync();

			return (pOut.Value == DBNull.Value) ? 0 : Convert.ToInt32(pOut.Value);
		}


        /// <summary>
        /// return -1 if user not found
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public async Task<int> Add(CorporateCustomer user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            await using var conn = await Connection.OpenAsync();

            using var cmd = new SqlCommand("dbo.AppCreateCorporateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 32).Value = user.Username;
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = user.PasswordHash;
            cmd.Parameters.Add("@CVR", SqlDbType.VarChar, 8).Value = user.CVR;
            cmd.Parameters.Add("@Credit", SqlDbType.Decimal).Value = user.Credit;           // fx 0m
            cmd.Parameters.Add("@InitialBalance", SqlDbType.Decimal).Value = user.Balance;  // fx 0m

            var pOut = cmd.Parameters.Add("@NewUserId", SqlDbType.Int);
            pOut.Direction = ParameterDirection.Output;

            if (await cmd.ExecuteNonQueryAsync() == 0){
                return -1;
            }

            return Convert.ToInt32(pOut.Value);
        }

        public async Task<User?> FindById(uint id)
        {
			await using var conn = await Connection.OpenAsync();

			// Hent én række for brugernavnet + om det er corporate, CPR/CVR og credit
			using var cmd = new SqlCommand(@"
                    SELECT TOP 1
                    u.userId,
                    u.username,
                    u.passwordHash,
                    u.balance,
                    CASE WHEN cc.userId IS NOT NULL THEN 1 ELSE 0 END AS IsCorporate,
                    pc.cpr,
                    cc.cvr,
                    cc.credit
                    FROM dbo.[user] u
                LEFT JOIN dbo.privateCustomer   pc ON pc.userId = u.userId
                LEFT JOIN dbo.corporateCustomer cc ON cc.userId = u.userId
                WHERE u.userId = @id;", conn);

			cmd.Parameters.Add("@id", SqlDbType.NVarChar, 32).Value = id;

			using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
			if (!await r.ReadAsync()) return null; // ukendt brugernavn

			// læs felter
			int userId = r.GetInt32(r.GetOrdinal("userId"));
			string uname = r.GetString(r.GetOrdinal("username"));
            string storedHash = r.GetString(r.GetOrdinal("passwordHash"));

			decimal balance = r.GetDecimal(r.GetOrdinal("balance"));
			bool isCorporate = r.GetInt32(r.GetOrdinal("IsCorporate")) == 1;

			if (isCorporate)
			{
				int ordCvr = r.GetOrdinal("cvr");
				int ordCredit = r.GetOrdinal("credit");

				string cvr = r.IsDBNull(ordCvr) ? "" : r.GetString(ordCvr);
				decimal credit = r.IsDBNull(ordCredit) ? 0m : r.GetDecimal(ordCredit);

				return new CorporateCustomer(
					id: (uint)userId,
					username: uname,
					passwordHash: storedHash,
					cvr: cvr,
					credit: credit,
					balance: balance
				);
			}
			else
			{
				int ordCpr = r.GetOrdinal("cpr");
				string cpr = r.IsDBNull(ordCpr) ? "" : r.GetString(ordCpr);

				return new PrivateCustomer(
					id: (uint)userId,
					username: uname,
					passwordHash: storedHash,
					cpr: cpr,
					balance: balance
				);
			}
		}

        public async Task<User?> LoginAsync(string username, string passwordPlain)
        {
            if (string.IsNullOrWhiteSpace(username) || passwordPlain is null)
                return null;

            await using var conn = await Connection.OpenAsync();

            // Hent én række for brugernavnet + om det er corporate, CPR/CVR og credit
            using var cmd = new SqlCommand(@"
                    SELECT TOP 1
                    u.userId,
                    u.username,
                    u.passwordHash,
                    u.balance,
                    CASE WHEN cc.userId IS NOT NULL THEN 1 ELSE 0 END AS IsCorporate,
                    pc.cpr,
                    cc.cvr,
                    cc.credit
                    FROM dbo.[user] u
                LEFT JOIN dbo.privateCustomer   pc ON pc.userId = u.userId
                LEFT JOIN dbo.corporateCustomer cc ON cc.userId = u.userId
                WHERE u.username = @Username;", conn);

            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 32).Value = username.Trim();

            using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (!await r.ReadAsync()) return null; // ukendt brugernavn

            // læs felter
            int userId = r.GetInt32(r.GetOrdinal("userId"));
            string uname = r.GetString(r.GetOrdinal("username"));
            string storedHash = r.GetString(r.GetOrdinal("passwordHash"));

            // verify password med din Domain.Hasher
            var hasher = new Hasher();
            if (!hasher.Verify(passwordPlain, storedHash))
                return null; // forkert password

            decimal balance = r.GetDecimal(r.GetOrdinal("balance"));
            bool isCorporate = r.GetInt32(r.GetOrdinal("IsCorporate")) == 1;

            if (isCorporate)
            {
                int ordCvr = r.GetOrdinal("cvr");
                int ordCredit = r.GetOrdinal("credit");

				string cvr = r.IsDBNull(ordCvr) ? "" : r.GetString(ordCvr);
                decimal credit = r.IsDBNull(ordCredit) ? 0m : r.GetDecimal(ordCredit);

                return new CorporateCustomer(
                    id: (uint)userId,
                    username: uname,
                    passwordHash: storedHash,
                    cvr: cvr,
                    credit: credit,
                    balance: balance
                );
            }
            else
            {
                int ordCpr = r.GetOrdinal("cpr");
                string cpr = r.IsDBNull(ordCpr) ? "" : r.GetString(ordCpr);

                return new PrivateCustomer(
                    id: (uint)userId,
                    username: uname,
                    passwordHash: storedHash,
                    cpr: cpr,
                    balance: balance
                );
            }
        }

        // (valgfrit) hurtig check til UI: findes brugernavn?
        public async Task<bool> UsernameExistsAsync(string username)
        {
            await using var conn = await Connection.OpenAsync();
            using var cmd = new SqlCommand("SELECT 1 FROM dbo.[user] WHERE username=@u;", conn);
            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 32).Value = username.Trim();
            var res = await cmd.ExecuteScalarAsync();
            return res is not null;
        }

        public async Task UpdateBalance(uint userId, decimal newBalance)
        {
            await using var conn = await Connection.OpenAsync();
            using var cmd = new SqlCommand("UPDATE [user] SET balance=@balance WHERE userId=@id;", conn);
            cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = newBalance;
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = (int)userId;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdatePassword(string passwordHash, uint userId)
        {
            await using var conn = await Connection.OpenAsync();
            using var cmd = new SqlCommand("UPDATE [user] SET passwordHash=@passwordHash WHERE userId=@id;", conn);
            cmd.Parameters.Add("@passwordHash", SqlDbType.NVarChar).Value = passwordHash;
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = (int)userId;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}


