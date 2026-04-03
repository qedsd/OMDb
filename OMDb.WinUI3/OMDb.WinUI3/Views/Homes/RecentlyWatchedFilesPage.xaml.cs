using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Interfaces;
using OMDb.WinUI3.ViewModels.Homes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views.Homes
{
    public sealed partial class RecentlyWatchedFilesPage : HomeItemBasePage,IHomeItem
    {
        public RecentlyWatchedFilesViewModel VM { get; set; } = new RecentlyWatchedFilesViewModel();
        public new static string ItemName = "最近观看视频";
        public RecentlyWatchedFilesPage()
        {
            this.InitializeComponent();
        }

        public async Task InitAsync()
        {
            await VM.InitAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.ItemClickCommand.Execute(e.ClickedItem);
        }

        private void MenuFlyoutItem1_Click(object sender, RoutedEventArgs e)
        {
            VM.ItemClickCommand.Execute((sender as FrameworkElement).DataContext);
        }

        private void MenuFlyoutItem2_Click(object sender, RoutedEventArgs e)
        {
            VM.ItemFolderCommand.Execute((sender as FrameworkElement).DataContext);
        }
    }
}
