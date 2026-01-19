using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWPasswordVault.WinUI.Services.Navigation
{
    public class NavigationService
    {
        Frame _contentFrame;
        public NavigationService(Frame contentFrame)
        {
            _contentFrame = contentFrame;
        }

        public void NavigateTo(string pageTag)
        {
            switch (pageTag)
            {
                case "home":
                    _contentFrame.Navigate(typeof(Views.HomePage));
                    break;
                case "Settings":
                    _contentFrame.Navigate(typeof(Views.SettingsPage));
                    break;
                case "login":
                    _contentFrame.Navigate(typeof(Views.LoginPage));
                    break;
                case "editPage":
                    _contentFrame.Navigate(typeof(Views.EditPage));
                    break;
                default:
                    break;
            }
        }
    }
}
