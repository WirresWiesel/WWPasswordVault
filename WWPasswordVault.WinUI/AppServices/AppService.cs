using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWPasswordVault.Core.Services.Authentication;
using WWPasswordVault.Core.Services.Crypt;
using WWPasswordVault.Core.Services.Hash;
using WWPasswordVault.Core.Services.Storage;
using WWPasswordVault.Core.Services.User;
using WWPasswordVault.WinUI.Services.Session;
using WWPasswordVault.WinUI.Services.VaultEntrys;

namespace WWPasswordVault.WinUI.AppServices
{
    public static class AppService
    {
        public static SessionService Session { get;  } = new SessionService();
        public static VaultEntrysService VaultEntrys { get; } = new VaultEntrysService();
    }
}
