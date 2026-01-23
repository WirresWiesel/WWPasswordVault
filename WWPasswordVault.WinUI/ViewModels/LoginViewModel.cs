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

namespace WWPasswordVault.WinUI.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        #region Commands
        public IRelayCommand LoginButton { get; }
        public IRelayCommand RegisterButton { get; }
        #endregion

        #region Observable Properties
        private string? _username;
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string? _confirmPassword;
        public string? ConfirmPassword
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
                    this.checkIfPasswordNeeded();
                }
            }
        }

        private bool _isLoginInfoBarOpen;
        public bool IsLoginInfoBarOpen
        {
            get => _isLoginInfoBarOpen;
            set => SetProperty(ref _isLoginInfoBarOpen, value);
        }

        private string? _infoMessage;
        public string? InfoMessage
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

        private string? _loginButtonText = "Login";
        public string? LoginButtonText
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
                if (SetProperty(ref _selectedUser, value))
                {
                    if (_selectedUser != null && value is AppUser user)
                    {
                        Username = user.Username;
                        user.HasCredentials = CoreService.Credentials.Exists(user.Username);
                        user.IsRemembered = user.HasCredentials;
                        RememberMe = user.IsRemembered;
                        IsRegisteredUser = true;
                        IsNeedingPassword = !user.IsRemembered;
                        OnPropertyChanged(nameof(Username));
                        OnPropertyChanged(nameof(RememberMe));
                        AppService.Session.SetCurrentUser(user);
                    }
                    else
                    {
                        IsNeedingPassword = true;
                    }
                }
            }
        }

        private bool _isNeedingPassword = true;
        public bool IsNeedingPassword
        {
            get => _isNeedingPassword;
            set => SetProperty(ref _isNeedingPassword, value);
        }

        private ObservableCollection<AppUser>? _users;
        public ObservableCollection<AppUser>? Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        #endregion

        public LoginViewModel()
        {
            LoginButton = new RelayCommand(OnLoginButtonClicked);
            RegisterButton = new RelayCommand(OnRegisterButtonClicked);
            this.updateRegisteredUsers();
        }

        private void OnLoginButtonClicked()
        {
            if (_isRegisterMode)
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
                this.updateRegisteredUsers();
                SelectedUser = _newUser;
                
                return;
            }

            Debug.WriteLine($"[Info] LoginViewModel: Login button clicked with Username: {Username} and Password: {Password}");
            bool _result = AppService.Session.UnlockSession(Username ?? string.Empty, Password ?? string.Empty, RememberMe);
            if (!_result)
            {
                InfoMessage = "Login Failed: \nInvalid username or password.";
                IsLoginInfoBarOpen = true;
                return;
            }
            else
            {
                var _currentUser = AppService.Session.CurrentUser;
                if (!_currentUser!.HasCredentials)
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

        private void OnRegisterButtonClicked()
        {
            bool _result = CoreService.Auth.UserExists(Username);
            Debug.WriteLine("[Info] LoginViewModel: Register button clicked.");
            IsLoginInfoBarOpen = true;
            if (_result)
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

        private void updateRegisteredUsers()
        {
            Users = new ObservableCollection<AppUser>(CoreService.Auth.GetRegisteredUsers());
        }

        private void checkIfPasswordNeeded()
        {
            if (_rememberMe == false)
            {
                Debug.WriteLine("Password needed.");
                IsNeedingPassword = true;
                return;
            }

            if (AppService.Session.CurrentUser != null)
            {
                Debug.WriteLine("User selected.");

                if (AppService.Session.CurrentUser.HasCredentials)
                {
                    Debug.WriteLine("User has credentials.");
                    IsNeedingPassword = false;
                }
                else
                {
                    Debug.WriteLine("User hasn´t credentials.");
                    IsNeedingPassword = true;
                }
            }
            else
            {
                Debug.WriteLine("No user selected.");
            }
        }
    }
}
