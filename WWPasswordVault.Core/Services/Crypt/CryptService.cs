using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WWPasswordVault.Core.Models;

namespace WWPasswordVault.Core.Services.Crypt
{
    public class CryptService
    {
        public void EncryptPasswordString(byte[] key, string plaintext, out byte[] ciphertext, out byte[] tag, out byte[] iv, out string version)
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

        public EncryptedValue EncryptVaultKey(byte[] KEK, byte[] vaultKey)
        {
            // Encryption logic to be implemented
            Debug.WriteLine("[Info] CryptService: Encryption.");
            byte[] _tag = new byte[16];
            byte[] _iv = RandomNumberGenerator.GetBytes(12);
            byte[] _vaultKey = vaultKey;
            byte[] _cipher = new byte[_vaultKey.Length];

            AesGcm aesGcm = new AesGcm(KEK, 16);
            aesGcm.Encrypt(_iv, _vaultKey, _cipher, _tag);

            string version = "1.0";

            EncryptedValue _encryptedValue = new EncryptedValue
            {
                _ciphertext = _cipher,
                _tag = _tag,
                _iv = _iv,
                _version = version
            };
            return _encryptedValue;
        }

        public void DecryptPasswordString(byte[] key, byte[] iv, byte[] ciphertext, byte[] tag, out string text)
        {
            // Decryption logic to be implemented
            Debug.WriteLine("[Info] CryptService: Decryption.");
            AesGcm aesGcm = new AesGcm(key, tag.Length);
            byte[] decrypted = new byte[ciphertext.Length];
            aesGcm.Decrypt(iv, ciphertext, tag, decrypted);
            text = Encoding.UTF8.GetString(decrypted);
        }

        public void DecryptVaultKey(byte[] encryptedKey, byte[] iv, byte[] ciphertext, byte[] tag, out byte[] key)
        {
            // Decryption logic to be implemented
            Debug.WriteLine("[Info] CryptService: Decryption.");
            AesGcm aesGcm = new AesGcm(encryptedKey, tag.Length);
            byte[] decrypted = new byte[ciphertext.Length];
            aesGcm.Decrypt(iv, ciphertext, tag, decrypted);
            key = decrypted;
        }
    }
}
