using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Services.Crypt
{
    public class CryptService
    {
        public void Encrypt(byte[] key, string plaintext, out byte[] ciphertext, out byte[] tag, out byte[] iv, out string version)
        {
            // Encryption logic to be implemented
            Debug.WriteLine("[Info] CryptService: Encryption.");
            byte[] _tag = new byte[16];
            byte[] _iv = RandomNumberGenerator.GetBytes(12);
            iv = _iv;
            byte[] _plaintext = Encoding.UTF8.GetBytes(plaintext);
            byte[] _ciphertext = new byte[_plaintext.Length];

            AesGcm aesGcm = new AesGcm(key, 16);
            aesGcm.Encrypt(_iv, _plaintext, _ciphertext, _tag);

            ciphertext = _ciphertext;
            tag = _tag;
            version = "1.0";
        }

        public void Decrypt(byte[] key, byte[] iv, byte[] ciphertext, byte[] tag, out string text)
        {
            // Decryption logic to be implemented
            Debug.WriteLine("[Info] CryptService: Decryption.");
            AesGcm aesGcm = new AesGcm(key, tag.Length);
            byte[] decrypted = new byte[ciphertext.Length];
            aesGcm.Decrypt(iv, ciphertext, tag, decrypted);
            text = Encoding.UTF8.GetString(decrypted);
        }
    }
}
