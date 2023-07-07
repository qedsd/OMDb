using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Events;
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
            var files = RecentFileService.RecentFiles;
            if(files.NotNullAndEmpty())
            {
                var list = new List<Core.Models.RecentEntry>();
                var groupByEntry = files.GroupBy(p => p.EntryId).ToList();
                foreach (var entryGroup in groupByEntry)
                {
                    var entry = await Core.Services.EntryService.QueryEntryAsync(new QueryItem(entryGroup.Key, entryGroup.First().DbId));
                    if(entry != null)
                    {
                        Core.Models.RecentEntry recentEntry = new Core.Models.RecentEntry();
                        recentEntry.RecentFile = entryGroup.OrderByDescending(p => p.AccessTime).First();
                        recentEntry.Entry = entry;
                        list.Add(recentEntry);
                    }
                }
                RecentlyWatchedEntries = list.ToObservableCollection();
            }
            else
            {
                RecentlyWatchedEntries = new ObservableCollection<RecentEntry>();
            }
            GlobalEvent.RecentFileChangedEvent += GlobalEvent_RecentFileChangedEvent;
        }

        private async void GlobalEvent_RecentFileChangedEvent(object sender, RecentFileChangedEventArgs e)
        {
            foreach(var file in e.RecentFiles)
            {
                var existedFile = RecentlyWatchedEntries.FirstOrDefault(p => p.Entry.EntryId == file.EntryId);
                if(existedFile != null)
                {
                    //已存在以前的观看记录里
                    existedFile.RecentFile = file;//当前更新的file更新时间肯定比以前任意一个时间要更晚
                    RecentlyWatchedEntries.Remove(existedFile);
                    RecentlyWatchedEntries.Insert(0, existedFile);
                }
                else
                {
                    //新增的观看记录
                    var entry = await Core.Services.EntryService.QueryEntryAsync(new QueryItem(file.EntryId, file.DbId));
                    if (entry != null)
                    {
                        Core.Models.RecentEntry recentEntry = new Core.Models.RecentEntry();
                        recentEntry.RecentFile = file;
                        recentEntry.Entry = entry;
                        RecentlyWatchedEntries.Insert(0, recentEntry);
                    }
                }
            }
        }
    }
}
