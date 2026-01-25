using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;
using WWPasswordVault.Core.CoreServices;
using WWPasswordVault.Core.Models;
using WWPasswordVault.WinUI.AppServices;

namespace WWPasswordVault.WinUI.Services.VaultEntrys
{
    public class VaultEntrysService
    {
        public ObservableCollection<string>? Categorys { get; set; }
        public List<VaultEntry> GetVaultEntrys()
        {
            Debug.WriteLine("[Info] VaultEntryService: Get vault entries.");
            return CoreService.JsonVaultStorage.GetVaultEntrys();
        }

        public ObservableCollection<VaultEntry> GetVaultEntriesByAppUserName(List<VaultEntry> vaultEntries, string name)
        {
            Debug.WriteLine("[Info] VaultEntryService: Get vault entries by AppUser.");
            ObservableCollection<VaultEntry> _appUserVaultEntries = new();
            foreach (VaultEntry entry in vaultEntries)
            {
                if (entry._appUser == AppService.Session.CurrentUser!.Username)
                {
                    _appUserVaultEntries.Add(entry);
                }
            }
            return _appUserVaultEntries;
        }

        public ObservableCollection<VaultEntry>? SelectVaultEntriesBySearchString(ObservableCollection<VaultEntry> appUserVaultEntries, string searchString)
        {
            Debug.WriteLine("[Info] VaultEntryService: Get vault entries by SearchString.");
            ObservableCollection<VaultEntry> _shownVaultEntries = new();
            if (!string.IsNullOrEmpty(searchString))
            {
                List<VaultEntry>? _tmpList = new List<VaultEntry>();
                _tmpList = appUserVaultEntries
                    .Where(s => s._title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (VaultEntry entry in _tmpList)
                {
                    _shownVaultEntries.Add(entry);
                }
            }
            else
            {
                return null;
            }

            return _shownVaultEntries;
        }

        public ObservableCollection<VaultEntry>? SelectVaultEntriesByCategory(ObservableCollection<VaultEntry> appUserVaultEntries, string? category)
        {
            Debug.WriteLine("[Info] VaultEntryService: Get vault entries by category.");
            ObservableCollection<VaultEntry> _shownVaultEntries = new();
            if (!string.IsNullOrEmpty(category))
            {
                List<VaultEntry>? _tmpList = new List<VaultEntry>();
                _tmpList = appUserVaultEntries
                    .Where(s => s._categoryList.Contains(category))
                    .ToList();

                foreach (VaultEntry entry in _tmpList)
                {
                    _shownVaultEntries.Add(entry);
                }
            }
            else
            {
                Debug.WriteLine("[Info] VaultEntryService: No category is given.");
                return null;
            }
            return _shownVaultEntries;
        }

        public ObservableCollection<string> GetCategorys(ObservableCollection<VaultEntry> appUserVaultEntries)
        {
            Debug.WriteLine("[Info] VaultEntryService: Get categorys.");
            //ObservableCollection<string> _shownCategorys = new();
            Categorys = new();
            foreach (VaultEntry entry in appUserVaultEntries)
            {
                entry.GetCategorysAsString();
                if (entry._categoryList == null)
                    break;

                foreach (string category in entry._categoryList)
                {
                    if (!Categorys.Contains(category))
                    {
                        Categorys.Add(category);
                    }
                }
            }

            return Categorys;
        }

        public bool DeleteVaultEntry(List<VaultEntry> appUserVaultEntries, VaultEntry vaultEntry)
        {
            Debug.WriteLine("[Info] VaultEntryService: Delete vault entry.");
            return appUserVaultEntries.Remove(vaultEntry);
        }
    }
}
