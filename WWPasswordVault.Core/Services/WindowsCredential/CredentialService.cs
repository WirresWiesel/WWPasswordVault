using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using WWPasswordVault.Core.CoreServices;
using WWPasswordVault.Core.Interfaces;
using WWPasswordVault.Core.Models;

namespace WWPasswordVault.Core.Services.WindowsCredential
{
    public class CredentialService : ICredentialService
    {
        private const string ResourcePrefix = "WWPasswordVault";

        private string GetResource(string username)
            =>$"{ResourcePrefix}:{username}";

        public void Save(string username, byte[] vaultKey)
        {
            Debug.WriteLine($"[Info] CredentialService: Saving credentials for user -> {username}");
            var vault = new PasswordVault();

            var credential = new PasswordCredential(
                GetResource(username),
                username,
                Convert.ToBase64String(vaultKey)
            );

            vault.Add( credential );
        }

        public byte[] Load(string username)
        {
            Debug.WriteLine($"[Info] CredentialService: Loading credentials for user -> {username}");
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve(
                    GetResource(username),
                    username);

                credential.RetrievePassword();

                byte[] _vaultKey = Convert.FromBase64String(credential.Password);

                return _vaultKey;
            }
            catch
            {
                return null;
            }
        }

        public void Delete(string username)
        {
            Debug.WriteLine($"[Info] CredentialService: Deleting credentials for user -> {username}");
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve(
                    GetResource(username),
                    username);

                vault.Remove(credential);
            }
            catch { }
        }

        public bool Exists(string username)
        {
            Debug.WriteLine($"[Info] CredentialService: Check credentials for user -> {username}");
            try
            {
                var vault = new PasswordVault();
                vault.Retrieve(GetResource(username), username);
                Debug.WriteLine("[Info] CredentialService: Credentials exists");
                return true;
            }
            catch
            {
                Debug.WriteLine("[Info] CredentialService: Credentials not exists");
                return false;
            }
        }
    }
}
