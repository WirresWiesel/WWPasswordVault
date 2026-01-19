using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Models
{
    public class EncryptedValue
    {
        public byte[] _ciphertext { get; set; }
        public byte[] _iv { get; set; }
        public byte[] _tag { get; set; }
        public string _version { get; set; }
    }
}
