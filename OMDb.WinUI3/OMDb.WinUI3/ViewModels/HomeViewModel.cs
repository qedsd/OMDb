using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.WinUI3.Extensions;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.Services.Settings;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class HomeViewModel: ObservableObject
    {

        public HomeViewModel()
        {
        }
        private void InitShowItem()
        {
            Views.HomePage.Current.ClearItem();
            foreach(var item in HomeItemConfigsService.ActiveItems)
            {
                Views.HomePage.Current.AddItem(Activator.CreateInstance(item.Type) as HomeItemBasePage);
            }
        }
        private async Task ItemInitAsync()
        {
            foreach (var item in Views.HomePage.Current.HomeItems)
            {
                await item.InitAsync();
            }
        }
        public async void Init()
        {
            Helpers.InfoHelper.ShowWaiting();
            InitShowItem();
            await ItemInitAsync();
            Helpers.InfoHelper.HideWaiting();
        }
        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            Helpers.InfoHelper.ShowWaiting();
            InitShowItem();
            await ItemInitAsync();
            Helpers.InfoHelper.HideWaiting();
        });
    }
}
