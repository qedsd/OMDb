using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels.Homes
{
    public class RandomEntryViewModel : ObservableObject
    {
        private ObservableCollection<Core.Models.Entry> randomEntries;
        public ObservableCollection<Core.Models.Entry> RandomEntries
        {
            get => randomEntries;
            set => SetProperty(ref randomEntries, value);
        }
        public async Task InitAsync()
        {
            var ls = await Core.Services.EntryService.RandomEntryAsync(10);
            if(ls.NotNullAndEmpty())
            {
                var queryItems = ls.Select(p => new Core.Models.QueryItem(p.EntryId, p.DbId)).ToList();
                var items = await Core.Services.EntryService.QueryEntryAsync(queryItems, true);
                if (items.NotNullAndEmpty())
                {
                    RandomEntries = items.ToObservableCollection();
                }
                else
                {
                    RandomEntries = null;
                }
            }
            else
            {
                RandomEntries = null;
            }
        }
        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            Helpers.InfoHelper.ShowWaiting();
            await InitAsync();
            Helpers.InfoHelper.HideWaiting();
        });
    }
}
