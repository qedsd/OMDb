using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using OMDb.Core.DbModels;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Dialogs;
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
                Desc = Entry?.Metadata?.Desc;
                Rating = Entry?.Entry.MyRating?? 0;
                Names = Entry?.Names?.DepthClone<ObservableCollection<EntryName>>();
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
        private double rating ;
        /// <summary>
        /// 编辑描述使用
        /// </summary>
        public double Rating
        {
            get => rating;
            set => SetProperty(ref rating, value);
        }
        private ObservableCollection<EntryName> names;
        /// <summary>
        /// 供编辑使用
        /// </summary>
        public ObservableCollection<EntryName> Names
        {
            get => names;
            set
            {
                SetProperty(ref names, value);
            }
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
            Entry.LoadLocalMetaData();
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
        private bool newHistorDone = true;
        public bool NewHistorDone
        {
            get => newHistorDone;
            set => SetProperty(ref newHistorDone, value);
        }
        private bool isEditWatchHistory = false;
        public bool IsEditWatchHistory
        {
            get => isEditWatchHistory;
            set=>SetProperty(ref isEditWatchHistory, value);
        }
        private Core.Models.WatchHistory EditingWatchHistory;
        public ICommand AddHistoryCommand => new RelayCommand(() =>
        {
            EditingWatchHistory = null;
            IsEditWatchHistory = true;
        });
        public ICommand SaveHistoryCommand => new RelayCommand(async() =>
        {
            if(EditingWatchHistory == null)//新增
            {
                Core.Models.WatchHistory watchHistory = new Core.Models.WatchHistory()
                {
                    Id = Guid.NewGuid().ToString(),
                    DbId = Entry.Entry.DbId,
                    EntryId = Entry.Entry.EntryId,
                    Done = NewHistorDone,
                    Time = new DateTime(NewHistorDate.Year, NewHistorDate.Month, NewHistorDate.Day, NewHistorTime.Hours, NewHistorTime.Minutes, 0),
                    Mark = NewHistorMark
                };
                Core.Services.EntryWatchHistoryService.AddWatchHistory(watchHistory);
                Entry.WatchHistory.Add(watchHistory);
            }
            else//编辑
            {
                EditingWatchHistory.Time = new DateTime(NewHistorDate.Year, NewHistorDate.Month, NewHistorDate.Day, NewHistorTime.Hours, NewHistorTime.Minutes, 0);
                EditingWatchHistory.Mark = NewHistorMark;
                EditingWatchHistory.Done = NewHistorDone;
                Core.Services.EntryWatchHistoryService.UpdateWatchHistory(EditingWatchHistory);
            }
            CancelEditHistoryCommand.Execute(null);
            await Entry.UpdateWatchHistoryAsync();//确保按时间倒序
            Helpers.InfoHelper.ShowSuccess("已保存");
        });
        public ICommand EditHistoryCommand => new RelayCommand<Core.Models.WatchHistory>((item) =>
        {
            if(item!=null)
            {
                EditingWatchHistory = item;
                NewHistorDate = new DateTimeOffset(item.Time);
                NewHistorMark = item.Mark;
                IsEditWatchHistory = true;
                NewHistorDone = item.Done;
            }
        });
        public ICommand DeleteHistoryCommand => new RelayCommand<Core.Models.WatchHistory>(async (item) =>
        {
            if (item != null)
            {
                if (await Dialogs.QueryDialog.ShowDialog("是否确认删除记录？", $"{item.Time} - {item.Mark}"))
                {
                    Core.Services.EntryWatchHistoryService.DeleteWatchHistory(item);
                    Entry.WatchHistory.Remove(item);
                    if(item.Done)
                    {
                        Entry.WatchCount -= 1;
                    }
                    Helpers.InfoHelper.ShowSuccess("已删除记录");
                }
            }
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
        public ICommand SaveRatingCommand => new RelayCommand<double>((value) =>
        {
            Rating = value;
            Entry.Entry.MyRating = Rating;
            Core.Services.EntryService.UpdateOrAddEntry(Entry.Entry);
            Helpers.InfoHelper.ShowSuccess("已更新评分");
        });
        public ICommand RefreshCommand => new RelayCommand(async() =>
        {
            Entry = await Models.EntryDetail.CreateAsync(Entry.Entry);
        });

        public ICommand SaveNamesCommand => new RelayCommand(async() =>
        {
            Names = Names.Where(p=>!string.IsNullOrEmpty(p.Name)).ToObservableCollection();
            await Core.Services.EntryNameSerivce.RemoveNamesAsync(Entry.Entry.EntryId, Entry.Entry.DbId);
            await Core.Services.EntryNameSerivce.AddNamesAsync(Names.Select(p => p.ToCoreEntryNameDb(Entry.Entry.EntryId)).ToList(), Entry.Entry.DbId);
            string oldEntryName = Entry.Name;
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Entry.Name = Names.FirstOrDefault(p => p.IsDefault)?.Name;
                Entry.Names = Names.DepthClone<ObservableCollection<EntryName>>();
            });
            if(oldEntryName != Names.FirstOrDefault(p => p.IsDefault)?.Name)
            {
                if(await Dialogs.QueryDialog.ShowDialog("词条名已修改","词条名已修改，是否重命名词条文件夹?"))
                {
                    int index = Entry.FullEntryPath.LastIndexOf(oldEntryName);
                    if (index > -1)
                    {
                        string newEntryPath = System.IO.Path.Combine(Entry.FullEntryPath.Substring(0, index), Entry.Name);
                        if (System.IO.Directory.Exists(newEntryPath))
                        {
                            await Dialogs.MsgDialog.ShowDialog("已存在同名文件夹，将维持原词条文件夹名", "移动失败");
                        }
                        else
                        {
                            Dialogs.WatingDialog.Show("移动文件中");
                            await Task.Run(() =>
                            {
                                Helpers.FileHelper.CopyFolder(Entry.FullEntryPath, newEntryPath, CopyFolderCallBack);
                            });
                            string newRelPath = System.IO.Path.Combine(Entry.Entry.Path.Substring(0, Entry.Entry.Path.LastIndexOf(oldEntryName)), Entry.Name);
                            Entry.Entry.Path = newRelPath;
                            Core.Services.EntryService.UpdateOrAddEntry(Entry.Entry);
                            Dialogs.WatingDialog.Hide();
                            RefreshCommand.Execute(null);
                        }
                    }
                }
            }
            Helpers.InfoHelper.ShowSuccess("已更新名称");
        });
        private int CopyFiles = 0;
        private void CopyFolderCallBack(float p)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Dialogs.WatingDialog.Show($"移动文件中(已移动文件:{++CopyFiles})");
            });
        }
        public ICommand CancelEidtNamesCommand => new RelayCommand(() =>
        {
            Names = Entry.Names.DepthClone<ObservableCollection<EntryName>>();
            Helpers.InfoHelper.ShowSuccess("已取消");
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
                    Entry.LoadLocalVideos();//前面添加的是无目录结构显示，需要刷新显示目录结构
                    Helpers.InfoHelper.ShowSuccess("复制完成");
                    if (VerifyFaiedVideoItems.Count!=0)//存在校验失败的
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
                    Entry.LoadLocalSubs();//前面添加的是无目录结构显示，需要刷新显示目录结构
                    Helpers.InfoHelper.ShowSuccess("复制完成");
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
                                Entry.MoreExplorerItems.Remove(s);
                            });
                        };
                        Entry.MoreExplorerItems.Add(p);
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
                    Entry.LoadLocalMore();//前面添加的是无目录结构显示，需要刷新显示目录结构
                    Helpers.InfoHelper.ShowSuccess("复制完成");
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

        #region 图片
        public ICommand DropImgCommand => new RelayCommand<IReadOnlyList<Windows.Storage.IStorageItem>>(async (items) =>
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
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetImgFolder());//创建目标文件路径
                    });
                    foreach (var p in sourceFiles)
                    {
                        await p.CopyAsync();
                    }
                    Entry.LoadLocalImgs();
                    Helpers.InfoHelper.ShowSuccess("复制完成");
                }
            }
        });
        #endregion

        #region 片单
        public ICommand AddToCollectionCommand => new RelayCommand(async() =>
        {
            var collections = await PickEntryCollectionDialog.ShowDialog();
            if(collections != null)
            {
                int addCount = 0;
                foreach (var collection in collections)
                {
                    if(Core.Services.EntryCollectionService.QueryFirst(collection.Id, Entry.Entry.EntryId) == null)
                    {
                        addCount++;
                        EntryCollectionItemDb entryCollectionItemDb = new EntryCollectionItemDb()
                        {
                            Id = Guid.NewGuid().ToString(),
                            EntryId = Entry.Entry.EntryId,
                            DbId = Entry.Entry.DbId,
                            CollectionId = collection.Id,
                            AddTime = DateTime.Now
                        };
                        Core.Services.EntryCollectionService.AddCollectionItem(entryCollectionItemDb);
                    }
                }
                if(addCount == 0)
                {
                    Helpers.InfoHelper.ShowError("已存在");
                }
                else
                {
                    Helpers.InfoHelper.ShowSuccess($"已添加到{addCount}个片单");
                }
            }
        });
        #endregion

        #region 台词
        private ObservableCollection<Core.Models.ExtractsLineBase> extractsLines;
        public ObservableCollection<Core.Models.ExtractsLineBase> ExtractsLines
        {
            get => extractsLines;
            set => SetProperty(ref extractsLines, value);
        }
        public ICommand AddLineCommand => new RelayCommand(async() =>
        {
            Dialogs.EditLineDialog editLineDialog = new EditLineDialog(null);
            if(await editLineDialog.ShowAsync())
            {
                Core.Models.ExtractsLineBase line = new Core.Models.ExtractsLineBase()
                {
                    Line = editLineDialog.Line,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                Entry.ExtractsLines.Add(line);
                Entry.Metadata.ExtractsLines = Entry.ExtractsLines.ToList();
                Entry.SaveMetadata();
                Helpers.InfoHelper.ShowSuccess("添加成功");
            }
        });
        public ICommand EditLineCommand => new RelayCommand<Core.Models.ExtractsLineBase>(async (item) =>
        {
            Dialogs.EditLineDialog editLineDialog = new EditLineDialog(item);
            if (await editLineDialog.ShowAsync())
            {
                item.Line = editLineDialog.Line;
                item.UpdateTime = DateTime.Now;
                Entry.ExtractsLines.Remove(item);
                Entry.ExtractsLines.Add(item);
                Entry.Metadata.ExtractsLines = Entry.ExtractsLines.ToList();
                Entry.SaveMetadata();
            }
        });
        public ICommand DeleteLineCommand => new RelayCommand<Core.Models.ExtractsLineBase>(async (item) =>
        {
            if(await Helpers.InfoHelper.ShowQueryAsync("是否确认删除选中的台词", item.Line))
            {
                Entry.ExtractsLines.Remove(item);
                Entry.Metadata.ExtractsLines = Entry.ExtractsLines.ToList();
                Entry.SaveMetadata();
            }
        });
        public ICommand LineDetailCommand => new RelayCommand<Core.Models.ExtractsLineBase>((item) =>
        {
            Dialogs.LineDetailDialog lineDetailDialog = new LineDetailDialog(item, Entry.Name);
            lineDetailDialog.Show();
        });
        #endregion
    }
}
