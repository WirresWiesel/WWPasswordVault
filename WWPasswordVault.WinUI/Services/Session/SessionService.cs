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

namespace WWPasswordVault.WinUI.Services.Session
{
    public class SessionService : ObservableObject
    {
        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        private AppUser? _currentUser;
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
                return false;
            }
        }

        public void SetCurrentUser(AppUser? user)
        {
            CurrentUser = user;
        }

        public AppUser? GetCurrentUser()
        {
            return CurrentUser;
        }
    }
}
