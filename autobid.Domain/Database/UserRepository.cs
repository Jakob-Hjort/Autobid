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
    internal class UserRepository
    {
        public async Task<int> Add(PrivateCustomer user)
        {
            await using var conn = await Connection.OpenAsync();

            using var cmd = new SqlCommand("dbo.AppCreatePrivateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("passwordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("cpr", user.CPR);
            cmd.Parameters.AddWithValue("balance", user.Balance);

            var pOut = new SqlParameter("@NewUserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pOut);

            await cmd.ExecuteNonQueryAsync();

            int newId = Convert.ToInt32(pOut.Value);
            // hvis din model har en Id-property:
            // user.UserId = newId;

            return newId;


        }

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

            await cmd.ExecuteNonQueryAsync();

            return Convert.ToInt32(pOut.Value);
        }




        public User? FindById(uint id)
        {
            throw new NotImplementedException();
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
                    credit: credit
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
                    cpr: cpr
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
    }
}


