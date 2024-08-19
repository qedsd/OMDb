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
    public partial class EntryDetailViewModel : ObservableObject
    {
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

        private bool isEditDesc = false;
        public bool IsEditDesc
        {
            get => isEditDesc;
            set => SetProperty(ref isEditDesc, value);
        }

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
                    sourceFiles.ForEach((Action<ExplorerItem>)(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetFolder(Services.ConfigService.VideoFolder));//创建目标文件路径
                        p.CanceledCopyEvent += (s) =>
                        {
                            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                            {
                                Entry.VideoExplorerItems.Remove(s);
                            });
                        };
                        Entry.VideoExplorerItems.Add(p);
                    }));
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
                    sourceFiles.ForEach((Action<ExplorerItem>)(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetFolder(Services.ConfigService.ImgFolder));//创建目标文件路径
                        p.CanceledCopyEvent += (s) =>
                        {
                            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                            {
                                Entry.SubExplorerItems.Remove(s);
                            });
                        };
                        Entry.SubExplorerItems.Add(p);
                    }));
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
                    sourceFiles.ForEach((Action<ExplorerItem>)(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetFolder(Services.ConfigService.ImgFolder));//创建目标文件路径
                        p.CanceledCopyEvent += (s) =>
                        {
                            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                            {
                                Entry.MoreExplorerItems.Remove(s);
                            });
                        };
                        Entry.MoreExplorerItems.Add(p);
                    }));
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
                    sourceFiles.ForEach((Action<ExplorerItem>)(p =>
                    {
                        p.SourcePath = p.FullName;//保留原文件路径
                        p.FullName = p.FullName.Replace(rootPath, Entry.GetFolder(Services.ConfigService.ImgFolder));//创建目标文件路径
                    }));
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
