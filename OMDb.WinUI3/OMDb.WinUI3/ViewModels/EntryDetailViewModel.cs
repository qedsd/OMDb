using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class EntryDetailViewModel : ObservableObject
    {
        private EntryDetail entry;
        public EntryDetail Entry
        {
            get => entry;
            set
            {
                SetProperty(ref entry, value);
                Desc = Entry.Metadata?.Desc;
            }
        }
        private string desc=string.Empty;
        /// <summary>
        /// 编辑描述使用
        /// </summary>
        public string Desc
        {
            get => desc;
            set => SetProperty(ref desc, value);
        }
        public EntryDetailViewModel(EntryDetail entry)
        {
            Entry = entry;
        }

        public ICommand OpenEntryPathCommand => new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start("explorer.exe", Entry.FullEntryPath);
        });

        private bool isEditDesc = false;
        public bool IsEditDesc
        {
            get => isEditDesc;
            set => SetProperty(ref isEditDesc, value);
        }
        public ICommand EditDescCommand => new RelayCommand(() =>
        {
            IsEditDesc = true;
        });
        public ICommand SaveDescCommand => new RelayCommand(() =>
        {
            CancelEditDescCommand.Execute(null);
            Entry.Metadata.Desc = Desc;
            Entry.Metadata.Save(System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.MetadataFileNmae));
            Entry.LoadMetaData();
        });
        public ICommand CancelEditDescCommand => new RelayCommand(() =>
        {
            IsEditDesc = false;
        });
        private DateTimeOffset newHistorDate = DateTimeOffset.Now;
        public DateTimeOffset NewHistorDate
        {
            get => newHistorDate;
            set => SetProperty(ref newHistorDate, value);
        }
        private TimeSpan newHistorTime=DateTimeOffset.Now.TimeOfDay;
        public TimeSpan NewHistorTime
        {
            get => newHistorTime;
            set => SetProperty(ref newHistorTime, value);
        }
        private string newHistorMark;
        public string NewHistorMark
        {
            get => newHistorMark;
            set => SetProperty(ref newHistorMark, value);
        }
        private bool isEditWatchHistory = false;
        public bool IsEditWatchHistory
        {
            get => isEditWatchHistory;
            set=>SetProperty(ref isEditWatchHistory, value);
        }
        public ICommand EditHistoryCommand => new RelayCommand(() =>
        {
            IsEditWatchHistory = true;
        });
        public ICommand SaveHistoryCommand => new RelayCommand(() =>
        {
            Core.Models.WatchHistory watchHistory = new Core.Models.WatchHistory()
            {
                DbId = Entry.Entry.DbId,
                Id = Entry.Entry.Id,
                Time = new DateTime(NewHistorDate.Year, NewHistorDate.Month, NewHistorDate.Day, NewHistorTime.Hours, NewHistorTime.Minutes, 0),
                Mark = NewHistorMark
            };
            Core.Services.WatchHistoryService.AddWatchHistoriesAsync(watchHistory);
            Entry.WatchHistory.Add(watchHistory);
            CancelEditHistoryCommand.Execute(null);
        });
        public ICommand CancelEditHistoryCommand => new RelayCommand(() =>
        {
            IsEditWatchHistory = false;
            NewHistorDate = DateTimeOffset.Now;
            NewHistorTime = DateTimeOffset.Now.TimeOfDay;
            NewHistorMark = null;
        });
        public ICommand OpenImgFolderCommand => new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.Combine(Entry.FullEntryPath,Services.ConfigService.ImgFolder));
        });
        public ICommand OpenVideoFolderCommand => new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.VideoFolder));
        });
        public ICommand OpenSubFolderCommand => new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.SubFolder));
        });
        public ICommand OpenResFolderCommand => new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.ResourceFolder));
        });

        public ICommand DropVideoCommand => new RelayCommand<IReadOnlyList<Windows.Storage.IStorageItem>>((items) =>
        {
            if(items?.Count > 0)
            {
                var paths = items.Select(p => p.Path).ToList();
            }
        });
    }
}
