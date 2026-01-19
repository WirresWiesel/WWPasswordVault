using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Interfaces
{
    public interface ICredentialService
    {
        void Save(string username, byte[] vaultKey);
        byte[] Load(string username);
        void Delete(string username);
        bool Exists(string username);
    }
}
