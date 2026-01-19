using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace WWPasswordVault.Core.Services.Data
{
    public class DataTransfer
    {
        public void CopyToClipboard(string textToCopy)
        {
            if (!string.IsNullOrEmpty(textToCopy))
            {
                var package = new DataPackage();
                package.SetText(textToCopy);
                Clipboard.SetContent(package);
                Clipboard.Flush();
            }
        }
    }
}
