using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Extensions;
using OMDb.Core.Models;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels.Homes
{
    public class RecentlyWatchedEntryViewModel : ObservableObject
    {
        private ObservableCollection<Core.Models.RecentEntry> recentlyWatchedEntries;
        public ObservableCollection<Core.Models.RecentEntry> RecentlyWatchedEntries
        {
            get => recentlyWatchedEntries;
            set => SetProperty(ref recentlyWatchedEntries, value);
        }

        public async Task InitAsync()
        {
            var files = await RecentFileService.GetRecentFilesAsync();
            var list = new List<Core.Models.RecentEntry>();
            var groupByEntry = files.GroupBy(p => p.EntryId).ToList();
            foreach (var entryGroup in groupByEntry)
            {
                Core.Models.RecentEntry recentEntry = new Core.Models.RecentEntry();
                recentEntry.RecentFile = entryGroup.OrderByDescending(p => p.AccessTime).First();
                recentEntry.Entry = await Core.Services.EntryService.QueryEntryAsync(new QueryItem(entryGroup.Key, entryGroup.First().DbId));
                list.Add(recentEntry);
            }
            RecentlyWatchedEntries = list.ToObservableCollection();
        }
    }
}
