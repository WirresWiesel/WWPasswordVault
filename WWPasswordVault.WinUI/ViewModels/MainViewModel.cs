using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WWPasswordVault.WinUI.Services.Navigation;
using Windows.Media.Capture.Frames;
using WWPasswordVault.WinUI.AppServices;
using WWPasswordVault.Core.CoreServices;
using WWPasswordVault.Core.Models;

namespace WWPasswordVault.WinUI.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private AppUser _user = new AppUser();
        public AppUser User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        private string _currentUser = string.Empty;
        public string CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private bool _commandBarState;
        public bool CommandBarState
        {
            get => _commandBarState;
            set => SetProperty(ref _commandBarState, value);
        }

        public IRelayCommand LogoutCommand { get; }

        public MainViewModel()
        {
            LogoutCommand = new RelayCommand(OnLogoutCommand);
        }

        private void OnLogoutCommand()
        {
            Debug.WriteLine("[Info] MainViewModel: Logout!");
            AppService.Session.LockSession();
        }

        public void UpdateMainViewModel()
        {
            Debug.WriteLine("[Info] MainViewModel: Updating MainViewModel.");
            User = AppService.Session.GetCurrentUser()!;
            if (User != null)
            {
                CurrentUser = User.Username;
                CommandBarState = true;
            }
            else
            {
                CurrentUser = string.Empty;
                CommandBarState = false;
            }
        }

    }
}
