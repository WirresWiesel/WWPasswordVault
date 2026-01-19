using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Services.User
{
    public class JsonStorageUserService
    {
        public List<Models.AppUser> Users { get; private set; } = new List<Models.AppUser>();

        private static string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WWPasswordVault", "Vault");
        private static string UserDataFilePath = Path.Combine(BasePath, "UserData.json");

        public JsonStorageUserService()
        {
            Debug.WriteLine("[Info] JsonStorageVaultService: Initializing JSON storage vault service.");
            Debug.WriteLine($"[Info] JsonStorageVaultService: Vault file path is {UserDataFilePath}");

            _initializeUserStorage();
        }

        private void _initializeUserStorage()
        {
            // Implementation for initializing vault storage
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
                Debug.WriteLine("[Info] JsonStorageVaultService: Created base directory for vault storage.");
            }

            if (File.Exists(UserDataFilePath))
            {
                Debug.WriteLine("[Info] JsonStorageVaultService: User file found. Loading user data.");
                _loadUserData();
            }
            else
            {
                Debug.WriteLine("[Info] JsonStorageVaultService: User file not found. Creating new user file.");
                _createEmptyUserFile();
            }
        }

        private void _createEmptyUserFile()
        {
            File.WriteAllText(UserDataFilePath, "[]", Encoding.UTF8);
            Debug.WriteLine("[Info] JsonStorageVaultService: Created empty vault file.");
        }

        private void _loadUserData()
        {
            // Implementation for loading user data from JSON file
            var jsonData = File.ReadAllText(UserDataFilePath, Encoding.UTF8);
            Users = System.Text.Json.JsonSerializer.Deserialize<List<Models.AppUser>>(jsonData) ?? new List<Models.AppUser>();
            Debug.WriteLine("[Info] JsonStorageVaultService: Loaded user data from file.");
        }

        public void SaveUserData(List<Models.AppUser> users)
        {
            // Implementation for saving user data to JSON file
            var jsonData = System.Text.Json.JsonSerializer.Serialize(users, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(UserDataFilePath, jsonData, Encoding.UTF8);
            Debug.WriteLine("[Info] JsonStorageVaultService: Saved user data to file.");
        }
    }
}
