using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using WWPasswordVault.WinUI.AppServices;
using WWPasswordVault.Core.CoreServices;
using WWPasswordVault.Core.Models;
using System.Security.Cryptography;

namespace WWPasswordVault.WinUI.Services.Session
{
    public class SessionService : ObservableObject
    {
        public byte[]? VaultKey { get; set; } = null;
        public byte[]? KEK { get; set; } = null;

        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        private AppUser? _currentUser = new();
        public AppUser? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public void LockSession()
        {
            IsLocked = true;
        }

        public bool UnlockSession(string Username, string Password, bool RememberMe = false)
        {
            if (CoreService.Auth.LoginUser(Username, Password, RememberMe))
            {
                IsLocked = false;
                return true;
            }
            else
            {
                LockSession();
                return false;
            }
        }

        public void SetCurrentUser(AppUser? user)
        {
            if (user == null)
            {
                CurrentUser = null;
            }
            CurrentUser = user;
        }

        public AppUser? GetCurrentUser()
        {
            return CurrentUser;
        }

        public void SetKEK(byte[] kek)
        {
            KEK = kek;
        }

        public void GenerateEncryptedVaultKey(AppUser user, byte[] KEK)
        {
            byte[] _vaultKey = RandomNumberGenerator.GetBytes(32);
            user.EncryptedVaultKey = CoreService.Crypt.EncryptVaultKey(KEK, _vaultKey);
            _vaultKey = new byte[32];
        }
    }
}
