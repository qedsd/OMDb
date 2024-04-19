using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Core;
using Microsoft.UI.Xaml.Shapes;
using NPOI.POIFS.FileSystem;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Helpers;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Extensions;
using OMDb.WinUI3.Services;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace OMDb.WinUI3.Models
{
    public partial class EntryDetail : ObservableObject
    {
        public EntryDetail() { }
        public EntryDetail(Core.Models.Entry entry)
        {
            this.Entry = entry.DepthClone<Core.Models.Entry>();
            this.Name = entry.Name;
            this.FullEntryPath = PathService.EntryFullPath(entry);
            this.FullCoverImgPath = PathService.EntryCoverImgFullPath(entry);
            this.FullMetaDataPath = System.IO.Path.Combine(FullEntryPath, Services.ConfigService.MetadataFileNmae);

            var storagePath = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.Name == entry.DbId).Path;

        }

        /// <summary>
        /// 读取本地资源
        /// </summary>
        public void LoadLocalResource()
        {
            LoadLocalVideos();
            LoadLocalSubs();
            LoadLocalMore();
            LoadLocalImgs();
        }




        public static async Task<EntryDetail> CreateAsync(Core.Models.Entry entry)
        {
            var entryDetail = new EntryDetail(entry);
            await entryDetail.Init();
            return entryDetail;
        }

        private void UpdateAlias()
        {
            if (Names != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var name in Names.Where(p => !p.IsDefault))
                {
                    stringBuilder.Append(name.Name);
                    if (!string.IsNullOrEmpty(name.Mark))
                    {
                        stringBuilder.Append($"({name.Mark})");
                    }
                    stringBuilder.Append(';');
                }
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }
                Alias = stringBuilder.ToString();
            }
            else
            {
                Alias = string.Empty;
            }
        }
        private async Task Init()
        {
            var namesT = await Core.Services.EntryNameSerivce.QueryNamesAsync(Entry.EntryId, Entry.DbId);
            if (namesT != null)
            {
                Names = namesT.Select(p => new EntryName(p)).ToObservableCollection();
            }
            await UpdateWatchHistoryAsync();
            _labels = await Core.Services.LabelService.GetLabelOfEntryAsync(Entry.EntryId);
            Labels ??= new List<Core.DbModels.LabelDb>();
            WatchHistory ??= new ObservableCollection<Core.Models.WatchHistory>();
            this.LoadResource();
        }
        public async Task UpdateWatchHistoryAsync()
        {
            var histories = (await Core.Services.EntryWatchHistoryService.QueryWatchHistoriesAsync(Entry.EntryId, Entry.DbId)).ToObservableCollection();
            if (histories == null)
            {
                WatchHistory = new ObservableCollection<Core.Models.WatchHistory>();
                WatchCount = 0;
            }
            else
            {
                WatchHistory = histories.OrderByDescending(p => p.Time).ToObservableCollection();
                WatchCount = WatchHistory.Where(p => p.Done).Count();
            }
        }
        public void LoadLocalImgs()
        {
            Imgs = new ObservableCollection<string>();
            string imgFolder = Path.Combine(FullEntryPath, Services.ConfigService.ImgFolder);
            if (Directory.Exists(imgFolder))
            {
                var items = Helpers.FileHelper.GetAllFiles(imgFolder);
                if (items != null && items.Any())
                {
                    foreach (var file in items)
                    {
                        if (ImageHelper.IsSupportImg(file.FullName))
                        {
                            Imgs.Add(file.FullName);
                        }
                    }
                }
            }
        }


        public void LoadLocalVideos()
        {
            string folder = Path.Combine(FullEntryPath, Services.ConfigService.VideoFolder);
            if (Directory.Exists(folder))
            {
                //VideoExplorerItems = Helpers.FileHelper.FindExplorerItems(folder).FirstOrDefault().Children?.ToObservableCollection();
                VideoExplorerItems = Helpers.FileHelper.GetAllFiles(folder).ToObservableCollection();
                if (VideoExplorerItems == null)
                {
                    VideoExplorerItems = new ObservableCollection<ExplorerItem>();
                }
            }
        }

        public void LoadLocalSubs()
        {
            string folder = Path.Combine(FullEntryPath, Services.ConfigService.SubFolder);
            if (Directory.Exists(folder))
            {
                SubExplorerItems = Helpers.FileHelper.FindExplorerItems(folder).FirstOrDefault().Children?.ToObservableCollection();
                if (SubExplorerItems == null)
                {
                    SubExplorerItems = new ObservableCollection<ExplorerItem>();
                }
            }
        }

        public void LoadLocalMore()
        {
            string folder = Path.Combine(FullEntryPath, Services.ConfigService.MoreFolder);
            if (Directory.Exists(folder))
            {
                MoreExplorerItems = Helpers.FileHelper.FindExplorerItems(folder).FirstOrDefault().Children?.ToObservableCollection();
                if (MoreExplorerItems == null)
                {
                    MoreExplorerItems = new ObservableCollection<ExplorerItem>();
                }
            }
        }


        public void LoadLines()
        {
            ExtractsLines = Entry.GetExtractsLines().ToObservableCollection();
            ExtractsLines ??= new ObservableCollection<Core.Models.ExtractsLineBase>();
        }

        public void LoadMetaData()
        {
            Metadata = Core.Models.EntryMetadata.Read(Path.Combine(FullEntryPath, Services.ConfigService.MetadataFileNmae));
        }


        /// <summary>
        /// 修改词条存储路径
        /// </summary>
        /// <param name="newPath"></param>
        public void ChangeEntryPath(string newPath)
        {

        }
        /// <summary>
        /// 修改词条封面
        /// </summary>
        /// <param name="newImgName">新封面图带后缀名名字，必须为词条Img文件夹下的图片文件</param>
        public void ChangeCoverImg(string newImgName)
        {
        }

        public string GetFolder(string relativePath)
        {
            return System.IO.Path.Combine(FullEntryPath, relativePath);
        }


        public bool SaveMetadata()
        {
            return Metadata.Save(System.IO.Path.Combine(FullEntryPath, Services.ConfigService.MetadataFileNmae));
        }

        /// <summary>
        /// 加載指定文件夾
        /// </summary>
        public void LoadPathFolderResource()
        {
            if (Directory.Exists(PathFolder))
            {
                var items = Helpers.FileHelper.FindExplorerItems(PathFolder).FirstOrDefault().Children?.ToObservableCollection();
                if (items != null && items.Count > 0)
                {
                    this.VideoExplorerItems = items.Where(
                        a => a.Name.EndsWith(".mp4") ||
                             a.Name.EndsWith(".mkv") ||
                             a.Name.EndsWith(".avi")
                        ).ToObservableCollection();
                    this.Imgs = items.Where(
                        a => a.Name.EndsWith(".jpg") ||
                             a.Name.EndsWith(".png") ||
                             a.Name.EndsWith(".jpeg")
                    ).Select(a => a.FullName).ToObservableCollection<string>();
                }

                if (this.VideoExplorerItems == null)
                    this.VideoExplorerItems = new ObservableCollection<ExplorerItem>();
                if (this.Imgs == null)
                    this.Imgs = new ObservableCollection<string>();
            }
        }

        private void LoadResource()
        {
            LoadLocalResource();
            LoadLines();
            LoadMetaData();
        }
    }
}
