using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.Core.Models
{
    public class VaultEntry
    {
        public string _appUser { get; set; }
        public string _title { get; set; }
        public string _username { get; set; }
        public List<string> _categoryList { get; set; }
        public string _categorys { get; set; }

        public EncryptedValue _password { get; set; }
        
        public void GetCategorysAsString()
        {
            var _outString = new StringBuilder();

            if (_categoryList != null)
            {
                for (int i = 0; i < _categoryList.Count; i++)
                {
                    _outString.Append(_categoryList[i]);

                    if (i < _categoryList.Count -1)
                        _outString.Append(", ");
                }
            }

            _categorys = _outString.ToString();
        }

        public void UpdateCategoryList()
        {
            List<string> categories = new();
            if (_categorys != null)
            {
                categories = _categorys.Split(",", StringSplitOptions.TrimEntries).ToList();
            }
            _categoryList = categories;
        }

        public void SetCategoryAsString(string categorys)
        {
            _categorys = categorys;
        }

    }
}
