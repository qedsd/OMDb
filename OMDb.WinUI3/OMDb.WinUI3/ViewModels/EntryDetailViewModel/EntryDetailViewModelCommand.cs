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
        public ICommand OpenFolderCommand => new RelayCommand<string>((type) =>
        {
            var path = string.Empty;
            switch (type)
            {
                case "Video":
                    path = System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.VideoFolder);
                    break;
                case "Image":
                    path = System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.ImgFolder);
                    break;
                case "Resource":
                    path = System.IO.Path.Combine(Entry.FullEntryPath, Services.ConfigService.ResourceFolder);
                    break;
                default:
                    path = Entry.FullEntryPath;
                    break;
            }
            System.Diagnostics.Process.Start("explorer.exe", Entry.FullEntryPath);
        });

        public ICommand SaveHistoryCommand => new RelayCommand(async () =>
        {
            if (EditingWatchHistory == null)//新增
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
            if (item != null)
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
                    if (item.Done)
                    {
                        Entry.WatchCount -= 1;
                    }
                    Helpers.InfoHelper.ShowSuccess("已删除记录");
                }
            }
        });

        public ICommand AddHistoryCommand => new RelayCommand(() =>
        {
            EditingWatchHistory = null;
            IsEditWatchHistory = true;
        });

        public ICommand CancelEditHistoryCommand => new RelayCommand(() =>
        {
            IsEditWatchHistory = false;
            NewHistorDate = DateTimeOffset.Now;
            NewHistorTime = DateTimeOffset.Now.TimeOfDay;
            NewHistorMark = null;
        });
 
        public ICommand SaveRatingCommand => new RelayCommand<double>((value) =>
        {
            Rating = value;
            Entry.Entry.MyRating = Rating;
            Core.Services.EntryService.UpdateOrAddEntry(Entry.Entry);
            Helpers.InfoHelper.ShowSuccess("已更新评分");
        });
        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            Entry = await Models.EntryDetail.CreateAsync(Entry.Entry);
        });

        public ICommand SaveNamesCommand => new RelayCommand(async () =>
        {
            Names = Names.Where(p => !string.IsNullOrEmpty(p.Name)).ToObservableCollection();
            await Core.Services.EntryNameSerivce.RemoveNamesAsync(Entry.Entry.EntryId, Entry.Entry.DbId);
            await Core.Services.EntryNameSerivce.AddNamesAsync(Names.Select(p => p.ToCoreEntryNameDb(Entry.Entry.EntryId)).ToList(), Entry.Entry.DbId);
            string oldEntryName = Entry.Name;
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Entry.Name = Names.FirstOrDefault(p => p.IsDefault)?.Name;
                Entry.Names = Names.DepthClone<ObservableCollection<EntryName>>();
            });
            if (oldEntryName != Names.FirstOrDefault(p => p.IsDefault)?.Name)
            {
                if (await Dialogs.QueryDialog.ShowDialog("词条名已修改", "词条名已修改，是否重命名词条文件夹?"))
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

    }
}
