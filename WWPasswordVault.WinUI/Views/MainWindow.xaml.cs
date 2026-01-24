using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using WWPasswordVault.WinUI.Views;
using WWPasswordVault.Core.Services.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using WWPasswordVault.WinUI.Services.Session;
using WWPasswordVault.WinUI.Services.Navigation;
using System.ComponentModel;
using WWPasswordVault.WinUI.AppServices;
using WWPasswordVault.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WWPasswordVault.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        NavigationService? _navService;
        MainViewModel _mainViewModel = new MainViewModel();

        public MainWindow()
        {
            Debug.WriteLine("[Info] MainWindow: Initializing MainWindow.");
            InitializeComponent();
            _navService = new NavigationService(ContentFrame);
            AppService.Session.PropertyChanged += OnSessionServicePropertyChanged;
            this.ExtendsContentIntoTitleBar = true;
            AppService.Session.LockSession();
            Root.DataContext = _mainViewModel;
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Debug.WriteLine("[Info] MainWindow: NavView_SelectionChanged -> function triggered from UI");
            if (args.SelectedItemContainer != null)
            {
                _navService!.NavigateTo(args.SelectedItemContainer.Tag.ToString() ?? string.Empty);
            }
        }

        private void OnSessionServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine("[Info] MainWindow: Something changed in Session.");
            if (sender == null)
                return;

            if (e.PropertyName == nameof(SessionService.IsLocked))
            {
                if (AppService.Session.IsLocked)
                {
                    Debug.WriteLine("[Info] MainWindow: Locking Session.");
                    MainNav.IsPaneVisible = false;
                    _navService!.NavigateTo("login");
                    MainNav.SelectedItem = null;
                    AppService.Session.SetCurrentUser(null);
                }
                else
                {
                    Debug.WriteLine("[Info] MainWindow: Unlocking Session.");
                    MainNav.IsPaneVisible = true;
                    MainNav.SelectedItem = MainNav.MenuItems
                        .OfType<NavigationViewItem>()
                        .FirstOrDefault(item => item.Tag?.ToString() == "home");
                }
                _mainViewModel.UpdateMainViewModel();
            }
        }
    }
}
