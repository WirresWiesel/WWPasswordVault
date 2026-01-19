using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using WWPasswordVault.Core.Models;
using WWPasswordVault.Core.Modifier;
using WWPasswordVault.Core.Services.Hash;
using WWPasswordVault.Core.Services.Storage;
using WWPasswordVault.Core.Services.User;
using WWPasswordVault.Core.Services.WindowsCredential;
using WWPasswordVault.Core.CoreServices;
using Windows.ApplicationModel.AppService;

namespace WWPasswordVault.Core.Services.Authentication
{
    public class AuthService
    {
        List<Models.AppUser> _registeredUsers = new List<Models.AppUser>();
        List<Models.VaultEntry> _availableVaults = new List<Models.VaultEntry>();

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

            CoreService.Hash.CreateHash(Password, null, out byte[] hash, out byte[] salt, out int iterations);
            // Future implementation for user registration
            Models.AppUser newUser = new Models.AppUser
            {
                Username = Username,
                Hash = hash,
                IsRemembered = RememberMe,
                Salt = salt,
                Iterations = iterations,
            };
            // Check if user already exists
            _registeredUsers.Add(newUser);
            CoreService.JsonUserStorage.SaveUserData(_registeredUsers);
            return newUser;
        }

        public bool LoginUser(string Username, string Password, bool RememberUser = false)
        {
            var user = _registeredUsers.FirstOrDefault(u => u.Username == Username);
            bool _credentialsSafed = CoreService.Credentials.Exists(Username);
            bool returnValue = false;

            if (user == null)
            {
                Debug.WriteLine("[Info] AuthService: User does not exist. Please Register!");
            }
            else
            {
                switch (StateModifier.BooleanToIntState(RememberUser, _credentialsSafed))
                {
                    case (int)StateModifier.State.PasswordLogin:
                        {
                            returnValue = this._passwordLogin(Password, user);
                        }
                        break;
                    case (int)StateModifier.State.CredentialLogin:
                        {
                            returnValue = this._credentialLogin(Password, user);
                        }
                        break;
                    case (int)StateModifier.State.DeleteCredentialThenPasswordLogin:
                        {
                            CoreService.Credentials.Delete(Username);
                            returnValue = this._passwordLogin(Password, user);
                        }
                        break;
                    case (int)StateModifier.State.SaveCredentialsThenCredentialLogin: // only when password is correct
                        {
                            if (CoreService.Hash.VerifyPassword(Password, user.Hash, user.Salt, user.Iterations))
                            {
                                Debug.WriteLine("[Info] AuthService: Create credentials and login.");
                                CoreService.Credentials.Save(Username, user.Hash);
                                returnValue = this._credentialLogin(Password, user);
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
            if (CoreService.Hash.VerifyPassword(password, appUser.Hash, appUser.Salt, appUser.Iterations))
            {
                Debug.WriteLine("[Info] AuthService: Login Successful.");
                return true;
            }
            return false;
        }

        private bool _credentialLogin(string password, AppUser appUser)
        {
            if (CoreService.Hash.VerifyPasswordKey(CoreService.Credentials.Load(appUser.Username), appUser.Hash))
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
    }
}
