using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WWPasswordVault.Core.Models;
using WWPasswordVault.Core.Services.Key;
using WWPasswordVault.WinUI.Services.Session;
using WWPasswordVault.WinUI.AppServices;
using System.Collections.ObjectModel;
using WWPasswordVault.Core.CoreServices;
using Microsoft.UI.Xaml.Controls;

namespace WWPasswordVault.WinUI.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        #region Commands
        public IRelayCommand LoginButton { get; }
        public IRelayCommand RegisterButton { get; }
        public IRelayCommand DeleteUserButton { get; }
        public IRelayCommand DeleteUserOnlyButton { get; }
        #endregion

        #region Observable Properties
        private string? _username;
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                if (SetProperty(ref _rememberMe, value))
                {
                    this.CheckIfPasswordNeeded();
                }
            }
        }

        private bool _isLoginInfoBarOpen;
        public bool IsLoginInfoBarOpen
        {
            get => _isLoginInfoBarOpen;
            set => SetProperty(ref _isLoginInfoBarOpen, value);
        }

        private string _infoMessage = string.Empty;
        public string InfoMessage
        {
            get => _infoMessage;
            set => SetProperty(ref _infoMessage, value);
        }

        private bool _isRegisterMode;
        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set
            {
                if (SetProperty(ref _isRegisterMode, value))
                {
                    LoginButtonText = _isRegisterMode ? "Register" : "Login";
                    OnPropertyChanged(nameof(LoginButtonText));
                }
            }
        }

        private bool _isRegisteredUser;
        public bool IsRegisteredUser
        {
            get => _isRegisteredUser;
            set => SetProperty(ref _isRegisteredUser, value);
        }

        private string _loginButtonText = "Login";
        public string LoginButtonText
        {
            get => _loginButtonText;
            set => SetProperty(ref _loginButtonText, value);
        }

        private object? _selectedUser;
        public object? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (!SetProperty(ref _selectedUser, value))
                    return;

                if (value != null)
                {
                    OnSelectUser(value);
                }
            }
        }

        private bool _isNeedingPassword = true;
        public bool IsNeedingPassword
        {
            get => _isNeedingPassword;
            set => SetProperty(ref _isNeedingPassword, value);
        }

        private ObservableCollection<AppUser> _users = new ();
        public ObservableCollection<AppUser> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        #endregion

        public LoginViewModel()
        {
            LoginButton = new RelayCommand(OnLoginButtonClicked);
            RegisterButton = new RelayCommand(OnRegisterButtonClicked);
            DeleteUserButton = new RelayCommand(OnDeleteUserClicked);
            DeleteUserOnlyButton = new RelayCommand(OnDeleteUserOnlyClicked);

            this.InitRegisteredUsers();
        }

        private void InitRegisteredUsers()
        {
            ObservableCollection<AppUser> tmpList = new ObservableCollection<AppUser>(CoreService.Auth.GetRegisteredUsers());
            Users = tmpList;
        }

        private void OnSelectUser(object value)
        {
            if (value is AppUser user)
            {
                Debug.WriteLine("[Info] LoginViewModel: Selected user changed.");
                Username = null;
                ConfirmPassword = string.Empty;
                user.HasCredentials = CoreService.Credentials.Exists(user.Username);
                user.IsRemembered = user.HasCredentials;

                RememberMe = user.IsRemembered;

                AppService.Session.SetCurrentUser(user);

                IsRegisteredUser = true;
                IsNeedingPassword = !user.IsRemembered;
                IsRegisterMode = false;
            }
            else
            {
                Debug.WriteLine("[Info] LoginViewModel: No registered user.");
                AppService.Session.SetCurrentUser(null);

                IsRegisteredUser = false;
                IsNeedingPassword = true;

                this.CleanUI();
            }
            IsLoginInfoBarOpen = false;
        }

        private void OnLoginButtonClicked()
        {
            IsLoginInfoBarOpen = false;
            if (IsRegisterMode)
            {
                //Check on registration if the two passwords are the same
                if (Password != ConfirmPassword)
                {
                    InfoMessage = "Registration Failed: \nPasswords do not match.";
                    IsLoginInfoBarOpen = true;
                    Debug.WriteLine("[Info] LoginViewModel: Passwords do not match");
                    return;
                }

                //Create/Register new user, then create the keys to safe the encryptedvault key in user.json
                AppUser _newUser = CoreService.Auth.RegisterUser(Username, Password, RememberMe);
                var _tmpKEK = CoreService.Auth.CreateKEK(_newUser, Password!);
                AppService.Session.SetKEK(_tmpKEK);
                AppService.Session.GenerateEncryptedVaultKey(_newUser, AppService.Session.KEK!);
                CoreService.Auth.SaveUser(_newUser);
                InfoMessage = "Registration done";
                IsLoginInfoBarOpen = true;
                IsRegisterMode = false;
                this.InitRegisteredUsers();
                SelectedUser = _newUser;
                
                return;
            }

            var _currentUser = AppService.Session.CurrentUser;
            if (_currentUser != null)
            {
                Debug.WriteLine($"[Info] LoginViewModel: Login button clicked with Username: {_currentUser.Username} and Password: {Password}");
                bool _result = AppService.Session.UnlockSession(_currentUser.Username ?? string.Empty, Password ?? string.Empty, RememberMe);
                if (!_result)
                {
                    InfoMessage = "Login Failed: \nInvalid username or password.";
                    IsLoginInfoBarOpen = true;
                    return;
                }
                else
                {
                    if (_currentUser != null && !_currentUser.HasCredentials)
                    {
                        var _tmpKEK = CoreService.Auth.CreateKEK(_currentUser, Password!);
                        AppService.Session.SetKEK(_tmpKEK);
                        CoreService.Crypt.DecryptVaultKey(AppService.Session.KEK, _currentUser!.EncryptedVaultKey._iv, _currentUser.EncryptedVaultKey._ciphertext, _currentUser.EncryptedVaultKey._tag, out byte[] vaultKey);
                        AppService.Session.VaultKey = vaultKey;
                    }
                    else
                    {
                        AppService.Session.VaultKey = CoreService.Credentials.Load(_currentUser!.Username);
                    }
                }
            }
            else
            {
                InfoMessage = "Login Failed: \nNo valid user.";
                IsLoginInfoBarOpen = true;
                return;
            }
        }

        private void OnRegisterButtonClicked()
        {
            Debug.WriteLine("[Info] LoginViewModel: Register button clicked.");
            IsLoginInfoBarOpen = true;
            if (CoreService.Auth.UserExists(Username) || AppService.Session.CurrentUser != null)
            {
                InfoMessage = "Registration Failed: \nUser already exists.";
                Debug.WriteLine("[Info] LoginViewModel: User already exist");
            }
            else
            {
                InfoMessage = "Confirm your password.";
                IsRegisterMode = true;
            }
        }

        private void OnDeleteUserClicked()
        {
            if (AppService.Session.CurrentUser == null)
                return;

            if (CoreService.Auth.VerifyUser(AppService.Session.CurrentUser, Password))
            {
                Debug.WriteLine("[Info] LoginViewModel: Delete user button clicked.");

                AppUser? _currentUser = new AppUser();
                _currentUser = AppService.Session.CurrentUser;
                this.FindAndDeleteCredentials(_currentUser);
                this.FindAndDeletePrograms(_currentUser);
                AppService.Session.CurrentUser = null;
                CoreService.Auth.DeleteUser(_currentUser);
                this.InitRegisteredUsers();

                // Workaround to avoid crash when the last entry in the list is deleted
                if (Users.Count > 0)
                {
                    SelectedUser = Users.First();
                }
                else
                {
                    SelectedUser = null;
                }

                if (Users != null)
                    CoreService.JsonUserStorage.SaveUserData(ConvertToList(Users));

                CleanUI();
            }
            else
            {
                InfoMessage = "Login Failed: \nInvalid username or password.";
                IsLoginInfoBarOpen = true;
            }
        }

        private void OnDeleteUserOnlyClicked()
        {
            if (AppService.Session.CurrentUser == null)
                return;

            if (CoreService.Auth.VerifyUser(AppService.Session.CurrentUser, Password))
            {
                Debug.WriteLine("[Info] LoginViewModel: Delete user button clicked.");

                AppUser _currentUser = AppService.Session.CurrentUser;
                this.FindAndDeleteCredentials(_currentUser);
                CoreService.Auth.DeleteUser(_currentUser);
                SelectedUser = null;
                this.InitRegisteredUsers();

                // Workaround to avoid crash when the last entry in the list is deleted
                if (Users.Count > 0)
                {
                    SelectedUser = Users.First();
                }
                else
                {
                    SelectedUser = null;
                }

                if (Users != null)
                    CoreService.JsonUserStorage.SaveUserData(ConvertToList(Users)); // list and obs. collection

                CleanUI();
            }
            else
            {
                InfoMessage = "Login Failed: \nInvalid username or password.";
                IsLoginInfoBarOpen = true;
            }
        }

        

        private void FindAndDeleteCredentials(AppUser user)
        {
            if (CoreService.Credentials.Exists(user.Username) && !string.IsNullOrEmpty(user.Username))
            {
                Debug.WriteLine("[Info] LoginViewModel: Delete found credentials.");
                CoreService.Credentials.Delete(user.Username);
            }
        }

        private void FindAndDeletePrograms(AppUser user)
        {
            if (AppService.Session.CurrentUser != null && string.IsNullOrEmpty(AppService.Session.CurrentUser.Username))
            {
                return;
            }
            Debug.WriteLine("[Info] LoginViewModel: Deleting all associated programs");
            List<VaultEntry> _tmpList = AppService.VaultEntrys.GetVaultEntrys();
            _tmpList.RemoveAll(entry => entry._appUser == AppService.Session.CurrentUser!.Username);
            CoreService.JsonVaultStorage.SaveVaultList(_tmpList);
        }

        private void CheckIfPasswordNeeded()
        {
            Debug.WriteLine("[Info] LoginViewModel: Check if password is needed.");
            if (_rememberMe == false)
            {
                Debug.WriteLine("[Info] LoginViewModel: Password needed.");
                IsNeedingPassword = true;
                return;
            }

            if (AppService.Session.CurrentUser != null)
            {

                if (AppService.Session.CurrentUser.HasCredentials)
                {
                    Debug.WriteLine("[Info] LoginViewModel: User has credentials.");
                    IsNeedingPassword = false;
                }
                else
                {
                    Debug.WriteLine("[Info] LoginViewModel: User hasn´t credentials. Needs password.");
                    IsNeedingPassword = true;
                }
            }
            else
            {
                Debug.WriteLine("[Info] LoginViewModel: No user selected.");
            }
        }

        private List<AppUser> ConvertToList(object toConvert)
        {
            List<AppUser> _tmpList = new();
            if (toConvert is ObservableCollection<AppUser>)
            {
                foreach (AppUser entry in (ObservableCollection<AppUser>)toConvert)
                {
                    _tmpList.Add(entry);
                }
            }
            return _tmpList;
        }

        private void CleanUI()
        {
            Debug.WriteLine("[Info] LoginViewModel: Cleaning UI.");

            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
}
