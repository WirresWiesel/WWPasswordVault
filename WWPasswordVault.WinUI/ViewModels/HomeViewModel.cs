using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using System.Diagnostics;
using WWPasswordVault.Core.CoreServices;
using WWPasswordVault.Core.Models;
using WWPasswordVault.WinUI.AppServices;
using Windows.ApplicationModel;
using System.ComponentModel;

namespace WWPasswordVault.WinUI.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        #region Propertys
        private string _searchString = string.Empty;
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (SetProperty(ref _searchString, value))
                {
                    Debug.WriteLine("[Info] HomeViewModel: SearchString changed.");
                    ShownVaultEntries = AppService.VaultEntrys.SelectVaultEntriesBySearchString(AppUserVaultEntries, _searchString) ?? AppUserVaultEntries;
                }
            }
        }

        private string? _shownPassword;
        public string? ShownPassword
        {
            get => _shownPassword;
            set => SetProperty(ref _shownPassword, value);
        }

        private bool _isOpenPopup = false;
        public bool IsOpenPopup
        {
            get => _isOpenPopup;
            set => SetProperty(ref _isOpenPopup, value);
        }

        private List<VaultEntry> _vaultEntries = new();
        public List<VaultEntry> VaultEntries
        {
            get => _vaultEntries;
            set => SetProperty(ref _vaultEntries, value);
        }

        private ObservableCollection<VaultEntry> _appUserVaultEntries = new();
        public ObservableCollection<VaultEntry> AppUserVaultEntries
        {
            get => _appUserVaultEntries;
            set => SetProperty(ref _appUserVaultEntries, value);
        }

        private ObservableCollection<VaultEntry> _shownVaultEntries = new();
        public ObservableCollection<VaultEntry> ShownVaultEntries
        {
            get => _shownVaultEntries;
            set => SetProperty(ref _shownVaultEntries, value);
        }

        private ObservableCollection<string> _categorys = new();
        public ObservableCollection<string> Categorys
        {
            get => _categorys;
            set => SetProperty(ref _categorys, value);
        }

        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    Debug.WriteLine($"[Info] HomeViewModel: SelectedCategory changed -> {_selectedCategory}");
                    ShownVaultEntries = AppService.VaultEntrys.SelectVaultEntriesByCategory(AppUserVaultEntries, _selectedCategory) ?? AppUserVaultEntries;
                }
            }
        }


        private VaultEntry? _selectedVaultEntry;
        public VaultEntry? SelectedVaultEntry
        {
            get => _selectedVaultEntry;
            set
            {
                if (SetProperty(ref _selectedVaultEntry, value))
                {
                    Debug.WriteLine("[Info] HomeViewModel: SelectedVaultEntry changed.");
                    ShownPassword = string.Empty;
                    ShowPassword = false;
                }
            }
        }

        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set
            {
                if (SetProperty(ref _showPassword, value))
                {
                    Debug.WriteLine("[Info] HomeViewModel: ShowPassword changed.");
                    this.OnShowPassword(_showPassword);
                }
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }
        #endregion

        #region Commands
        public IRelayCommand? CopyCommand { get; }
        public IRelayCommand? DeleteCommand { get; }
        public IRelayCommand? EditCommand { get;  }
        public IRelayCommand? SaveCommand { get; }
        #endregion

        public HomeViewModel()
        {
            Debug.WriteLine("[Info] HomeViewModel: Initializing HomeViewModel.");
            CopyCommand = new RelayCommand(OnCopyCommand);
            DeleteCommand = new RelayCommand(OnDeleteCommand);
            EditCommand = new RelayCommand(OnEditEntryCommand);
            SaveCommand = new RelayCommand(OnSaveCommand);
            VaultEntries = AppService.VaultEntrys.GetVaultEntrys();
            AppUserVaultEntries = AppService.VaultEntrys.GetVaultEntriesByAppUserName(VaultEntries, AppService.Session.CurrentUser!.Username);
            ShownVaultEntries = AppUserVaultEntries;
            Categorys = AppService.VaultEntrys.GetCategorys(AppUserVaultEntries);
        }

        private void UpdateVaultEntries()
        {
            Debug.WriteLine("[Info] HomeViewModel: Update vault entries.");
            AppUserVaultEntries = AppService.VaultEntrys.GetVaultEntriesByAppUserName(VaultEntries, AppService.Session.CurrentUser!.Username);
            string _savedCategory = SelectedCategory ?? string.Empty;
            VaultEntry? _savedSelectedVaultEntry = SelectedVaultEntry ?? null;
            Categorys = AppService.VaultEntrys.GetCategorys(AppUserVaultEntries);

            if (Categorys.Contains(_savedCategory!))
            {
                SelectedCategory = _savedCategory;
            }

            if (SelectedCategory != null)
            {
                ShownVaultEntries = AppService.VaultEntrys.SelectVaultEntriesByCategory(AppUserVaultEntries, _selectedCategory) ?? AppUserVaultEntries;
            }
            else
            {
                Debug.WriteLine("[Info] HomeViewModel: Resut ShownVaultEntries to AppUserVaultEntries.");
                ShownVaultEntries = AppUserVaultEntries;
            }

            if (_savedSelectedVaultEntry != null)
            {
                SelectedVaultEntry = _savedSelectedVaultEntry;
            }
        }

        private void OnShowPassword(bool state)
        {
            Debug.WriteLine("[Info] HomeViewModel: Show password.");
            if (SelectedVaultEntry != null && state)
            {
                ShownPassword = string.Empty;
                ShownPassword = this.getPassword();
            }
            else
            {
                ShownPassword = string.Empty;
            }
        }

        private void OnCopyCommand()
        {
            Debug.WriteLine("[Info] HomeViewModel: Copy password to clipboard.");
            CoreService.DataTransfer.CopyToClipboard(this.getPassword());
        }

        private string getPassword()
        {
            Debug.WriteLine("[Info] HomeViewModel: Get password.");
            AppUser _currentUser = AppService.Session.CurrentUser ?? new AppUser();

            if (SelectedVaultEntry != null)
            {
                CoreService.Crypt.DecryptPasswordString(AppService.Session.VaultKey, SelectedVaultEntry._password._iv, SelectedVaultEntry._password._ciphertext, SelectedVaultEntry._password._tag, out string _password);
                return _password;
            }
            return string.Empty;
        }

        private void OnDeleteCommand()
        {
            if (AppService.VaultEntrys.DeleteVaultEntry(VaultEntries, SelectedVaultEntry!))
            {
                Debug.WriteLine("[Info] HomeViewModel: Entry deleted");
            }
            this.UpdateVaultEntries();
            CoreService.JsonVaultStorage.SaveVaultEntries();
        }

        private void  OnEditEntryCommand()
        {
            Debug.WriteLine("[Info] HomeViewModel: EditEntryCommand clicked.");
            Debug.WriteLine($"[Info] HomeViewModel: Is edit mode = {IsEditMode}.");
        }

        private void OnSaveCommand()
        {
            Debug.WriteLine("[Info] HomeViewModel: OnSaveCommand clicked.");

            if (SelectedVaultEntry != null)
            {
                var rootEntry = VaultEntries.FirstOrDefault(p => p._title == SelectedVaultEntry._title);
                if (rootEntry != null)
                {
                    rootEntry.UpdateCategoryList();
                }

                if (!string.IsNullOrEmpty(ShownPassword))
                {
                    CoreService.Crypt.EncryptPasswordString(AppService.Session.VaultKey, ShownPassword, out byte[] _ciphertext, out byte[] _tag, out byte[] _iv, out string _version);
                    SelectedVaultEntry._password._ciphertext = _ciphertext;
                    SelectedVaultEntry._password._tag = _tag;
                    SelectedVaultEntry._password._iv = _iv;
                    SelectedVaultEntry._password._version = _version;
                }

                CoreService.JsonVaultStorage.SaveVaultEntries();
            }

            this.UpdateVaultEntries();
            IsEditMode = false;
        }
    }
}
