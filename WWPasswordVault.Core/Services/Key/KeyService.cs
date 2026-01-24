using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace WWPasswordVault.Core.Services.Key
{
    public class KeyService
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int DefaultIterations = 600_000;

        public byte[] CreateKey(string password, byte[] passwordSalt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, passwordSalt, DefaultIterations, HashAlgorithmName.SHA256);

            return pbkdf2.GetBytes(KeySize);
        }

        public bool VerifyString(string password, byte[] storedKey, byte[] storedSalt)
        {
            byte[] computedHash = Array.Empty<byte>();
            if (password != null)
            {
                using var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, DefaultIterations, HashAlgorithmName.SHA256);
                computedHash = pbkdf2.GetBytes(KeySize);
            }
            return computedHash.SequenceEqual(storedKey);
        }

        public bool VerifyKey(byte[] key, byte[] storedKey)
        {
            if (!key.SequenceEqual(storedKey))
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
