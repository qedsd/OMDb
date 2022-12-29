using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
    public sealed partial class RandomEntryPage : HomeItemBasePage, IHomeItem
    {
        public RandomEntryViewModel VM { get; set; } = new RandomEntryViewModel();
        public RandomEntryPage()
        {
            this.InitializeComponent();
        }

        public async Task InitAsync()
        {
            await VM.InitAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.EntryDetailPage), e.ClickedItem);
        }
    }
}
