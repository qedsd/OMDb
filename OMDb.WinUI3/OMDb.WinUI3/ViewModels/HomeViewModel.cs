using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using OMDb.Core.Extensions;
using OMDb.Core.Models;
using OMDb.WinUI3.Services;
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
        private ObservableCollection<Core.Models.RecentFile> recentlyWatchedFiles;
        public ObservableCollection<Core.Models.RecentFile> RecentlyWatchedFiles
        {
            get => recentlyWatchedFiles;
            set => SetProperty(ref recentlyWatchedFiles, value);
        }

        private ObservableCollection<Core.Models.RecentEntry> recentlyWatchedEntries;
        public ObservableCollection<Core.Models.RecentEntry> RecentlyWatchedEntries
        {
            get => recentlyWatchedEntries;
            set => SetProperty(ref recentlyWatchedEntries, value);
        }

        private ObservableCollection<Core.Models.Entry> recentlyUpdatedEntries;
        public ObservableCollection<Core.Models.Entry> RecentlyUpdatedEntries
        {
            get => recentlyUpdatedEntries;
            set => SetProperty(ref recentlyUpdatedEntries, value);
        }
        private ImageSource lineCover;
        /// <summary>
        /// 台词封面
        /// </summary>
        public ImageSource LineCover
        {
            get => lineCover;
            set => SetProperty(ref lineCover, value);
        }

        private ExtractsLine extractsLine;
        /// <summary>
        /// 台词内容
        /// </summary>
        public ExtractsLine ExtractsLine
        {
            get => extractsLine;
            set => SetProperty(ref extractsLine, value);
        }

        public HomeViewModel()
        {
            Init();
        }
        private async void Init()
        {
            Helpers.InfoHelper.ShowWaiting();
            await InitRecentlyWatchedFiles();
            await InitRecentlyWatchedEntries();
            await InitRecentlyUpdatedEntries();
            Helpers.InfoHelper.HideWaiting();
        }
        private async Task InitRecentlyWatchedFiles()
        {
            var files = await RecentFileService.GetRecentFilesAsync();
            if(files != null && files.Any())
            {
                RecentlyWatchedFiles = files.ToObservableCollection();
            }
        }
        private async Task InitRecentlyWatchedEntries()
        {
            if (RecentlyWatchedFiles != null && RecentlyWatchedFiles.Any())
            {
                var list = new List<Core.Models.RecentEntry>();
                var groupByEntry = RecentlyWatchedFiles.GroupBy(p => p.EntryId).ToList();
                foreach(var entryGroup in groupByEntry)
                {
                    Core.Models.RecentEntry recentEntry = new Core.Models.RecentEntry();
                    recentEntry.RecentFile = entryGroup.OrderByDescending(p => p.AccessTime).First();
                    recentEntry.Entry = await Core.Services.EntryService.QueryEntryAsync(new QueryItem(entryGroup.Key, entryGroup.First().DbId));
                    list.Add(recentEntry);
                }
                RecentlyWatchedEntries = list.ToObservableCollection();
            }
        }
        private async Task InitRecentlyUpdatedEntries()
        {
            var items = await Core.Services.EntryService.QueryEntryAsync(Core.Enums.SortType.LastUpdateTime,Core.Enums.SortWay.Reverse);
            if(items.NotNullAndEmpty())
            {
                var targetItems = items.Take(7).Select(p=>p.ToQueryItem()).ToList();
                var entries = await Core.Services.EntryService.QueryEntryAsync(targetItems);
                if(entries.NotNullAndEmpty())
                {
                    RecentlyUpdatedEntries = entries.ToObservableCollection();
                }
            }
        }

        private async Task SetExtractsLine()
        {
            Core.Services.EntryService
        }
    }
}
