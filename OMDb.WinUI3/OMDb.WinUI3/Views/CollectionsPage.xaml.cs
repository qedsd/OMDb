using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionsPage : Page
    {
        public CollectionsPage()
        {
            this.InitializeComponent();
        }

        private void MenuFlyoutItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CollectionsViewModel).EditCommand.Execute((sender as FrameworkElement).DataContext);
        }

        private void MenuFlyoutItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CollectionsViewModel).RemoveCommand.Execute((sender as FrameworkElement).DataContext);
        }
    }
}
