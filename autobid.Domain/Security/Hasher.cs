using autobid.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Security
{
	public class Hasher : IPasswordHasher
	{
		private const int SaltSize = 16; // bytes
		private const int KeySize = 32;  // bytes (256-bit)
		private const int Iterations = 100_000;

		public string Hash(string text)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(text);

			using var rng = RandomNumberGenerator.Create();
			byte[] salt = new byte[SaltSize];
			rng.GetBytes(salt);

			using Rfc2898DeriveBytes derive = new(text, salt, Iterations, HashAlgorithmName.SHA256);
			byte[] key = derive.GetBytes(KeySize);

			// store as: iterations.salt.key (base64)
			return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
		}

		public bool Verify(string password, string hashed)
		{
			var parts = hashed.Split('.', 3);
			if (parts.Length != 3) return false;

			int iterations = int.Parse(parts[0]);
			byte[] salt = Convert.FromBase64String(parts[1]);
			byte[] key = Convert.FromBase64String(parts[2]);

			using var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
			byte[] keyToCheck = derive.GetBytes(key.Length);

			return CryptographicOperations.FixedTimeEquals(key, keyToCheck);
		}
	}
}
