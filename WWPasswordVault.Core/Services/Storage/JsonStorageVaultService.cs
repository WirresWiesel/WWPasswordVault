using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWPasswordVault.Core.Models;

namespace WWPasswordVault.Core.Services.Storage
{
    public class JsonStorageVaultService
    {
        private List<Models.VaultEntry> VaultEntries { get; set; } = new List<Models.VaultEntry>();

        private static string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WWPasswordVault", "Vault");
        private static string VaultDataFilePath = Path.Combine(BasePath, "VaultData.json");

        public JsonStorageVaultService()
        {
            Debug.WriteLine("[Info] JsonStorageVaultService: Initializing JSON storage vault service.");
            Debug.WriteLine($"[Info] JsonStorageVaultService: Vault file path is {VaultDataFilePath}");

            _initializeVaultStorage();
        }

        private void _initializeVaultStorage()
        {
            // Implementation for initializing vault storage
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
                Debug.WriteLine("[Info] JsonStorageVaultService: Created base directory for vault storage.");
            }

            if (File.Exists(VaultDataFilePath))
            {
                Debug.WriteLine("[Info] JsonStorageVaultService: Vault file found. Loading vault data.");
                _loadVaultEntries();
            }
            else
            {
                Debug.WriteLine("[Info] JsonStorageVaultService: Vault file not found. Creating new vault file.");
                _createEmptyVaultFile();
            }
        }

        private void _createEmptyVaultFile()
        {
            File.WriteAllText(VaultDataFilePath, "[]", Encoding.UTF8);
            Debug.WriteLine("[Info] JsonStorageVaultService: Created empty vault file.");
        }

        private void _loadVaultEntries()
        {
            // Implementation for loading vault entries from JSON file
            var jsonData = File.ReadAllText(VaultDataFilePath, Encoding.UTF8);
            VaultEntries = System.Text.Json.JsonSerializer.Deserialize<List<Models.VaultEntry>>(jsonData) ?? new List<Models.VaultEntry>();
            Debug.WriteLine("[Info] JsonStorageVaultService: Loaded vault entries from file.");
        }

        public void SaveVaultEntries()
        {
            // Implementation for saving vault entries to JSON file
            var jsonData = System.Text.Json.JsonSerializer.Serialize(VaultEntries, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(VaultDataFilePath, jsonData, Encoding.UTF8);
            Debug.WriteLine("[Info] JsonStorageVaultService: Saved vault entries to file.");
        }

        public void SaveVaultList(List<VaultEntry> _tmpList)
        {
            // Implementation for saving vault entries to JSON file
            var jsonData = System.Text.Json.JsonSerializer.Serialize(_tmpList, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(VaultDataFilePath, jsonData, Encoding.UTF8);
            Debug.WriteLine("[Info] JsonStorageVaultService: Saved vault entries to file.");
        }

        public void AddVaultEntry(Models.VaultEntry vaultEntry)
        {
            VaultEntries.Add(vaultEntry);
        }

        public List<Models.VaultEntry> GetVaultEntrys()
        {
            return VaultEntries;
        }
    }
}
