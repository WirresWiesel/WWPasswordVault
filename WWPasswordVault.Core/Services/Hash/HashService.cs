using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace WWPasswordVault.Core.Services.Hash
{
    public class HashService
    {
        private const int SaltSize = 16; // 128 bit
        private const int HashSize = 32; // 256 bit
        private const int DefaultIterations = 600_000;

        public void CreateHash(string password, int? customIterations, out byte[] hash, out byte[] salt, out int iterations)
        {
            if (customIterations == null)
            {
                iterations = DefaultIterations;
            }
            else
            {
                iterations = customIterations.Value;
            }

            salt = RandomNumberGenerator.GetBytes(SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);

            hash = pbkdf2.GetBytes(HashSize);
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt, int storedIterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, storedIterations, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(HashSize);
            return computedHash.SequenceEqual(storedHash);
        }

        public bool VerifyPasswordKey(byte[] key, byte[] storedHash)
        {
            if (!key.SequenceEqual(storedHash))
            {
                Debug.WriteLine("[Info] HashService: Invalid Key. Try Again");
                return false;
            }
            else
            {
                Debug.WriteLine("[Info] HashService: Valid Key. Logging in");
                return true;
            }
        }
    }
}
