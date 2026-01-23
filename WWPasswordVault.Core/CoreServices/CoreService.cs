using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWPasswordVault.Core.Services.Authentication;
using WWPasswordVault.Core.Services.Crypt;
using WWPasswordVault.Core.Services.Key;
using WWPasswordVault.Core.Services.Storage;
using WWPasswordVault.Core.Services.User;
using WWPasswordVault.Core.Services.WindowsCredential;
using WWPasswordVault.Core.Services.Data;

namespace WWPasswordVault.Core.CoreServices
{
    public static class CoreService
    {
        public static JsonStorageVaultService JsonVaultStorage { get; } = new JsonStorageVaultService();
        public static JsonStorageUserService JsonUserStorage { get; } = new JsonStorageUserService();
        public static AuthService Auth { get; } = new AuthService();
        public static CryptService Crypt { get; } = new CryptService();
        public static KeyService Key { get; } = new KeyService();
        public static CredentialService Credentials{ get; } = new CredentialService();
        public static DataTransfer DataTransfer { get; } = new DataTransfer();
    }
}
