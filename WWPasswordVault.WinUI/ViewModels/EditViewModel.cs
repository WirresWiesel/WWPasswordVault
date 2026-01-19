using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWPasswordVault.Core.CoreServices;
using WWPasswordVault.Core.Models;
using WWPasswordVault.WinUI.AppServices;

namespace WWPasswordVault.WinUI.ViewModels
{
    public class EditViewModel : ObservableObject
    {
        #region Commands
        public IRelayCommand SaveButton { get; }
        public IRelayCommand CancelButton { get; }
        #endregion

        #region Propertys
        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string? _username;
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string? _category;
        public string? Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private AppUser? _currentUser;
        public AppUser? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private ObservableCollection<string>? _savedCategorys;
        public ObservableCollection<string>? SavedCategorys
        {
            get => _savedCategorys;
            set => SetProperty(ref _savedCategorys, value);
        }
        
        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private string? _newCategory;
        public string? NewCategory
        {
            get => _newCategory;
            set => SetProperty(ref _newCategory, value);
        }
        #endregion

        public EditViewModel()
        {
            SaveButton = new RelayCommand(OnSaveButton);
            CancelButton = new RelayCommand(OnCancelButton);
            CurrentUser = AppService.Session.GetCurrentUser();
            this.transferCategorys();
        }

        public void OnSaveButton()
        {
            if (CurrentUser == null)
                return;

            CoreService.Crypt.Encrypt(CurrentUser.Hash, Password, out byte[] ciphertext, out byte[] tag, out byte[] iv, out string version);
            EncryptedValue _encValue = new()
            {
                _ciphertext = ciphertext,
                _iv = iv,
                _tag = tag,
                _version = version
            };

            VaultEntry newEntry = new()
            {
                _appUser = CurrentUser.Username,
                _title = Title,
                _username = Username,
                _category = SelectedCategory,
                _password = _encValue
            };
            // Save logic here
            CoreService.JsonVaultStorage.AddVaultEntry(newEntry);
            CoreService.JsonVaultStorage.SaveVaultEntries();
            this.clearTextBoxes();
        }

        public void OnCancelButton()
        {
            // Cancel logic here
        }

        private void clearTextBoxes()
        {
            Username = string.Empty;
            Title = string.Empty;
            Category = string.Empty;
            Password = string.Empty;
        }

        private void transferCategorys()
        {
            SavedCategorys = new();
            foreach( string entry in AppService.VaultEntrys.Categorys)
            {
                SavedCategorys.Add(entry);
            }
        }
    }
}
