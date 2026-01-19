using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Models
{
    public class VaultEntry
    {
        public string _appUser { get; set; }
        public string _title { get; set; }
        public string _username { get; set; }
        public string _category { get; set; }
        public EncryptedValue _password { get; set; }
    }
}
