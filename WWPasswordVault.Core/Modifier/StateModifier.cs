using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Modifier
{
    public static class StateModifier
    {
        public enum State
        {
            PasswordLogin,
            CredentialLogin,
            SaveCredentialsThenCredentialLogin,
            DeleteCredentialThenPasswordLogin
        }

        public static int BooleanToIntState(bool firstState, bool secondState)
        {
            return (firstState, secondState) switch
            {
                (true, true) => (int)State.CredentialLogin,
                (true, false) => (int)State.SaveCredentialsThenCredentialLogin,
                (false, true) => (int)State.DeleteCredentialThenPasswordLogin,
                (false, false) => (int)State.PasswordLogin
            };
        }
    }
}
