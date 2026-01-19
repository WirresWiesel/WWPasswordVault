using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Models
{
    public class LoginParameters
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginParameters(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
