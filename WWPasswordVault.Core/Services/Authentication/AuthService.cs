using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using WWPasswordVault.Core.Models;
using WWPasswordVault.Core.Modifier;
using WWPasswordVault.Core.Services.Key;
using WWPasswordVault.Core.Services.Storage;
using WWPasswordVault.Core.Services.User;
using WWPasswordVault.Core.Services.WindowsCredential;
using WWPasswordVault.Core.CoreServices;
using Windows.ApplicationModel.AppService;
using System.Security.Cryptography;

namespace WWPasswordVault.Core.Services.Authentication
{
    public class AuthService
    {
        private List<Models.AppUser> _registeredUsers = new List<Models.AppUser>();
        private List<Models.VaultEntry> _availableVaults = new List<Models.VaultEntry>();

        public AuthService()
        {
            _initializeAuthService();
        }

        private void _initializeAuthService()
        {
            // Future implementation for initializing authentication service
            _registeredUsers = CoreService.JsonUserStorage.Users;
            _availableVaults = CoreService.JsonVaultStorage.GetVaultEntrys();
        }

        public AppUser RegisterUser(string Username, string Password, bool RememberMe)
        {
            if (UserExists(Username))
            {
                return null;
            }

            // Create a new user and set basic informations and random salts
            Models.AppUser newUser = new Models.AppUser
            {
                Username = Username,
                IsRemembered = RememberMe,
                PasswordKey = null,
                PasswordSalt = RandomNumberGenerator.GetBytes(32),
                VaultKeySalt = RandomNumberGenerator.GetBytes(32),
                EncryptedVaultKey = null
            };

            // Set key for later verification at login
            newUser.PasswordKey = CoreService.Key.CreateKey(Password, newUser.PasswordSalt);
            return newUser;
        }

        public bool LoginUser(string Username, string Password, bool RememberUser = false)
        {
            var user = _registeredUsers.FirstOrDefault(u => u.Username == Username);
            bool returnValue = false;

            if (user == null)
            {
                Debug.WriteLine("[Info] AuthService: User does not exist. Please Register!");
            }
            else
            {
                switch (StateModifier.BooleanToIntState(RememberUser, user.HasCredentials))
                {
                    case (int)StateModifier.State.PasswordLogin:
                        {
                            returnValue = this._passwordLogin(Password, user);
                        }
                        break;
                    case (int)StateModifier.State.CredentialLogin:
                        {
                            returnValue = this._credentialLogin(user);
                        }
                        break;
                    case (int)StateModifier.State.DeleteCredentialThenPasswordLogin:
                        {
                            CoreService.Credentials.Delete(Username);
                            user.HasCredentials = false;
                            returnValue = this._passwordLogin(Password, user);
                        }
                        break;
                    case (int)StateModifier.State.SaveCredentialsThenCredentialLogin:
                        {
                            if (CoreService.Key.VerifyString(Password, user.PasswordKey, user.PasswordSalt))
                            {
                                Debug.WriteLine("[Info] AuthService: Create credentials and login.");
                                var _tmpKEK = CoreService.Key.CreateKey(Password, user.VaultKeySalt);
                                CoreService.Crypt.DecryptVaultKey(_tmpKEK, user.EncryptedVaultKey._iv, user.EncryptedVaultKey._ciphertext, user.EncryptedVaultKey._tag, out byte[] vaultKey);
                                CoreService.Credentials.Save(user.Username, vaultKey);
                                returnValue = this._credentialLogin(user);
                            }
                            else
                            {
                                Debug.WriteLine("[Info] AuthService: Can´t log in because password is incorrect.");
                            }
                        }
                        break;
                }
                _updateRememberMeStatus(user, RememberUser);
            }
            return returnValue;
        }

        public bool UserExists(string Username)
        {
            var user = _registeredUsers.FirstOrDefault(u => u.Username == Username);
            if (user != null)
            {
                Debug.WriteLine($"[Info] AuthService: User {Username} already exists. Please Login!");
                return true;
            }
            return false;
        }

        private void _updateRememberMeStatus(Models.AppUser User, bool RememberUser)
        {
            if (RememberUser != User.IsRemembered)
            {
                User.IsRemembered = RememberUser;
                Debug.WriteLine($"[Info] AuthService: Updated RememberUser status for {User.Username} to {RememberUser}.");
                CoreService.JsonUserStorage.SaveUserData(_registeredUsers);
            }
        }

        private bool _passwordLogin(string password, AppUser appUser)
        {
            if (CoreService.Key.VerifyString(password, appUser.PasswordKey, appUser.PasswordSalt))
            {
                Debug.WriteLine("[Info] AuthService: Login Successful.");
                return true;
            }
            return false;
        }

        private bool _credentialLogin(AppUser appUser)
        {
            if (CoreService.Credentials.Exists(appUser.Username))
            {
                Debug.WriteLine("[Info] AuthService: Login via Windows Credentials Sucessful.");
                return true;
            }
            return false;
        }

        public List<Models.AppUser> GetRegisteredUsers()
        {
            return _registeredUsers;
        }

        public void SaveUser(AppUser user)
        {
            _registeredUsers.Add(user);
            CoreService.JsonUserStorage.SaveUserData(_registeredUsers);
        }

        public byte[] CreateKEK(AppUser user, string password)
        {
            return CoreService.Key.CreateKey(password, user.VaultKeySalt);
        }
    }
}
