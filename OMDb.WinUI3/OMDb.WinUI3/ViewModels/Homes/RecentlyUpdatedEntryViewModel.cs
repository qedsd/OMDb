using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels.Homes
{
    public class RecentlyUpdatedEntryHomeViewModel : ObservableObject
    {
        private ObservableCollection<Core.Models.Entry> recentlyUpdatedEntries;
        public ObservableCollection<Core.Models.Entry> RecentlyUpdatedEntries
        {
            get => recentlyUpdatedEntries;
            set => SetProperty(ref recentlyUpdatedEntries, value);
        }
        public async Task InitAsync()
        {
            await InitRecentlyUpdatedEntries();
        }
        private async Task InitRecentlyUpdatedEntries()
        {
            var sortModel = new SortModel(Core.Enums.SortType.LastUpdateTime, Core.Enums.SortWay.Reverse);
            var filterModel = new FilterModel();
            var items = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
            if (items.NotNullAndEmpty())
            {
                var targetItems = items.Take(7).Select(p => p.ToQueryItem()).ToList();
                var entries = await Core.Services.EntryService.QueryEntryAsync(targetItems);
                if (entries.NotNullAndEmpty())
                {
                    RecentlyUpdatedEntries = entries.ToObservableCollection();
                }
            }
        }
    }
}
