using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Models
{
    public class AppUser
    {
        //User
        public string Username { get; set; }
        public string Email { get; set; }
        //Login
        public bool IsRemembered { get; set; }
        public bool HasCredentials { get; set; }
        public byte[] PasswordKey { get; set; }
        public byte[] PasswordSalt { get; set; }
        //VaultKey
        public byte[] VaultKeySalt { get; set; }
        public EncryptedValue EncryptedVaultKey {  get; set; }
    }
}
