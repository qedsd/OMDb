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
        #region 视频
        public ICommand DropVideoCommand => new RelayCommand<IReadOnlyList<Windows.Storage.IStorageItem>>(async(items) =>
        {
            if(items?.Count > 0)
            {
                var paths = items.Select(p => p.Path).ToList();
                List<ExplorerItem> source = new List<ExplorerItem>();//源文件，保留原本目录结构
                foreach (var path in paths)
                {
                    source.AddRange(Helpers.FileHelper.FindExplorerItems(path));
                }
                if(source.Count > 0)
                {
                    string rootPath = System.IO.Path.GetDirectoryName(source.First().FullName);//要复制的文件的公共根路径
                    var sourceFiles = Helpers.FileHelper.GetAllFiles(source);//每一个都是文件
                    sourceFiles.ForEach(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath,Entry.GetVideoFolder());//创建目标文件路径
                        p.CanceledCopyEvent += (s) =>
                        {
                            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                            {
                                Entry.VideoExplorerItems.Remove(s);
                            });
                        };
                        Entry.VideoExplorerItems.Add(p);
                    });
                    if (VerifyFaiedVideoItems == null)
                    {
                        VerifyFaiedVideoItems = new List<ExplorerItem>();
                    }
                    else
                    {
                        VerifyFaiedVideoItems.Clear();
                    }
                    ExplorerItem.VerifyFaiedEvent += VideoExplorerItem_VerifyFaiedEvent;
                    foreach(var p in sourceFiles)
                    {
                        await p.CopyAsync();
                    }
                    ExplorerItem.VerifyFaiedEvent -= VideoExplorerItem_VerifyFaiedEvent;
                    Entry.LoadVideos();//前面添加的是无目录结构显示，需要刷新显示目录结构
                    if(VerifyFaiedVideoItems.Count!=0)//存在校验失败的
                    {
                        await Dialogs.ExplorerItemVerifyFaiedDialog.ShowDialog(VerifyFaiedVideoItems, Entry.FullEntryPath);
                    }
                }
            }
        });
        private List<ExplorerItem> VerifyFaiedVideoItems;
        private void VideoExplorerItem_VerifyFaiedEvent(ExplorerItem explorerItem)
        {
            Helpers.InfoHelper.ShowError($"{explorerItem.Name}校验不通过");
            VerifyFaiedVideoItems.Add(explorerItem);
        }
        #endregion

        #region 字幕
        public ICommand DropSubCommand => new RelayCommand<IReadOnlyList<Windows.Storage.IStorageItem>>(async (items) =>
        {
            if (items?.Count > 0)
            {
                var paths = items.Select(p => p.Path).ToList();
                List<ExplorerItem> source = new List<ExplorerItem>();//源文件，保留原本目录结构
                foreach (var path in paths)
                {
                    source.AddRange(Helpers.FileHelper.FindExplorerItems(path));
                }
                if (source.Count > 0)
                {
                    string rootPath = System.IO.Path.GetDirectoryName(source.First().FullName);//要复制的文件的公共根路径
                    var sourceFiles = Helpers.FileHelper.GetAllFiles(source);//每一个都是文件
                    sourceFiles.ForEach(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetSubFolder());//创建目标文件路径
                        p.CanceledCopyEvent += (s) =>
                        {
                            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                            {
                                Entry.SubExplorerItems.Remove(s);
                            });
                        };
                        Entry.SubExplorerItems.Add(p);
                    });
                    if (VerifyFaiedSubItems == null)
                    {
                        VerifyFaiedSubItems = new List<ExplorerItem>();
                    }
                    else
                    {
                        VerifyFaiedSubItems.Clear();
                    }
                    ExplorerItem.VerifyFaiedEvent += SubExplorerItem_VerifyFaiedEvent;
                    foreach (var p in sourceFiles)
                    {
                        await p.CopyAsync();
                    }
                    ExplorerItem.VerifyFaiedEvent -= SubExplorerItem_VerifyFaiedEvent;
                    Entry.LoadSubs();//前面添加的是无目录结构显示，需要刷新显示目录结构
                    if (VerifyFaiedSubItems.Count != 0)//存在校验失败的
                    {
                        await Dialogs.ExplorerItemVerifyFaiedDialog.ShowDialog(VerifyFaiedSubItems, Entry.FullEntryPath);
                    }
                }
            }
        });
        private List<ExplorerItem> VerifyFaiedSubItems;
        private void SubExplorerItem_VerifyFaiedEvent(ExplorerItem explorerItem)
        {
            Helpers.InfoHelper.ShowError($"{explorerItem.Name}校验不通过");
            VerifyFaiedSubItems.Add(explorerItem);
        }
        #endregion

        #region 资源
        public ICommand DropResCommand => new RelayCommand<IReadOnlyList<Windows.Storage.IStorageItem>>(async (items) =>
        {
            if (items?.Count > 0)
            {
                var paths = items.Select(p => p.Path).ToList();
                List<ExplorerItem> source = new List<ExplorerItem>();//源文件，保留原本目录结构
                foreach (var path in paths)
                {
                    source.AddRange(Helpers.FileHelper.FindExplorerItems(path));
                }
                if (source.Count > 0)
                {
                    string rootPath = System.IO.Path.GetDirectoryName(source.First().FullName);//要复制的文件的公共根路径
                    var sourceFiles = Helpers.FileHelper.GetAllFiles(source);//每一个都是文件
                    sourceFiles.ForEach(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetResFolder());//创建目标文件路径
                        p.CanceledCopyEvent += (s) =>
                        {
                            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                            {
                                Entry.ResExplorerItems.Remove(s);
                            });
                        };
                        Entry.ResExplorerItems.Add(p);
                    });
                    if (VerifyFaiedResItems == null)
                    {
                        VerifyFaiedResItems = new List<ExplorerItem>();
                    }
                    else
                    {
                        VerifyFaiedResItems.Clear();
                    }
                    ExplorerItem.VerifyFaiedEvent += ResExplorerItem_VerifyFaiedEvent;
                    foreach (var p in sourceFiles)
                    {
                        await p.CopyAsync();
                    }
                    ExplorerItem.VerifyFaiedEvent -= ResExplorerItem_VerifyFaiedEvent;
                    Entry.LoadRes();//前面添加的是无目录结构显示，需要刷新显示目录结构
                    if (VerifyFaiedResItems.Count != 0)//存在校验失败的
                    {
                        await Dialogs.ExplorerItemVerifyFaiedDialog.ShowDialog(VerifyFaiedResItems, Entry.FullEntryPath);
                    }
                }
            }
        });
        private List<ExplorerItem> VerifyFaiedResItems;
        private void ResExplorerItem_VerifyFaiedEvent(ExplorerItem explorerItem)
        {
            Helpers.InfoHelper.ShowError($"{explorerItem.Name}校验不通过");
            VerifyFaiedResItems.Add(explorerItem);
        }
        #endregion
    }
}
