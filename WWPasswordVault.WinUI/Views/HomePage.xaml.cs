using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WWPasswordVault.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            Debug.WriteLine("[Info] HomePage: Initializing HomePage.");
            InitializeComponent();
            this.DataContext = new ViewModels.HomeViewModel();

            RootGrid.Tapped += RootGrid_Tapped;
        }
        
        //Necessary code behind because there is no possible way to push the event via the xaml to the view model
        //Is only working when the grid background ist transparant and has a name like "RootGrid"
        private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("[Info] HomePage: RootGrid.Tapped event.");
            // DataContext als ViewModel casten
            if (this.DataContext is ViewModels.HomeViewModel vm)
            {
                // OriginalSource gibt das tatsächliche Element zurück
                var clickedElement = e.OriginalSource;

                // Prüfen, ob kein ListViewItem geklickt wurde
                if (clickedElement is Grid)
                {
                    Debug.WriteLine("[Info] HomePage: Grid was tapped on empty space.");
                    vm.SelectedVaultEntry = null;
                    vm.SelectedCategory = null;

                    e.Handled = true;
                }
            }
        }
    }
}
