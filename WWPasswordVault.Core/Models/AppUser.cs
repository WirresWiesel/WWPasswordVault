using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Models
{
    public class AppUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsRemembered { get; set; }
        public bool HasCredentials { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Salt { get; set; }
        public int Iterations { get; set; }
    }
}
