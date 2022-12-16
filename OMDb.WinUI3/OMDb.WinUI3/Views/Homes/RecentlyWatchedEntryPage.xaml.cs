using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Models;
using OMDb.WinUI3.Interfaces;
using OMDb.WinUI3.Services;
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
    public sealed partial class RecentlyWatchedEntryPage : HomeItemBasePage, IHomeItem
    {
        public RecentlyWatchedEntryViewModel VM { get; set; } = new RecentlyWatchedEntryViewModel();
        public new static string ItemName = "最近观看词条";
        public RecentlyWatchedEntryPage()
        {
            this.InitializeComponent();
        }

        public async Task InitAsync()
        {
            await VM.InitAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.EntryDetailPage), (e.ClickedItem as RecentEntry).Entry);
        }
    }
}
