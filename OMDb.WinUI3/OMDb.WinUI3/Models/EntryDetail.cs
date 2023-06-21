using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Core;
using OMDb.Core.Enums;
using OMDb.Core.Extensions;
using OMDb.Core.Models;
using OMDb.Core.Services;
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

namespace OMDb.WinUI3.Models
{
    public class EntryDetail : ObservableObject
    {
        public EntryDetail() { }
        public EntryDetail(Core.Models.Entry entry)
        {
            Entry = entry.DepthClone<Core.Models.Entry>();
            Name = entry.Name;
            FullEntryPath = PathService.EntryFullPath(entry);
            FullCoverImgPath = PathService.EntryCoverImgFullPath(entry);
            FullMetaDataPath = System.IO.Path.Combine(FullEntryPath, Services.ConfigService.MetadataFileNmae);

            var result = EntrySourceSerivce.SelectEntrySource(entry.EntryId, entry.DbId);
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId).StoragePath;

            switch (entry.SaveType)
            {
                case SaveType.Folder:
                    {
                        if (s != null && !string.IsNullOrEmpty(entry.Path))
                            PathFolder = s + result.Where(a => a.PathType == PathType.Folder).FirstOrDefault().Path;
                        break;
                    }

                case SaveType.Files:
                    {
                        if (s != null && !string.IsNullOrEmpty(entry.Path))
                        {
                            PathImg = result.Where(a => a.PathType == PathType.Image).Select(a => s + a.Path).ToObservableCollection();
                            PathVideo = result.Where(a => a.PathType == PathType.Video).Select(a => s + a.Path).ToObservableCollection();
                            PathAudio = result.Where(a => a.PathType == PathType.VideoSub).Select(a => s + a.Path).ToObservableCollection();
                            PathMore = result.Where(a => a.PathType == PathType.More).Select(a => s + a.Path).ToObservableCollection();
                        }
                        break;
                    }
                case SaveType.Local:
                    break;
                default:
                    break;
            }
        }

        private string _pathFolder = string.Empty;
        public string PathFolder
        {
            get => _pathFolder;
            set => SetProperty(ref _pathFolder, value);
        }

        private ObservableCollection<string> _pathImg = new ObservableCollection<string>();
        public ObservableCollection<string> PathImg
        {
            get => _pathImg;
            set => SetProperty(ref _pathImg, value);
        }
        private ObservableCollection<string> _pathVideo = new ObservableCollection<string>();
        public ObservableCollection<string> PathVideo
        {
            get => _pathVideo;
            set => SetProperty(ref _pathVideo, value);
        }
        private ObservableCollection<string> _pathAudio = new ObservableCollection<string>();
        public ObservableCollection<string> PathAudio
        {
            get => _pathAudio;
            set => SetProperty(ref _pathAudio, value);
        }

        private ObservableCollection<string> _pathMore = new ObservableCollection<string>();
        public ObservableCollection<string> PathMore
        {
            get => _pathMore;
            set => SetProperty(ref _pathMore, value);
        }


        public static async Task<EntryDetail> CreateAsync(Core.Models.Entry entry)
        {
            var entryDetail = new EntryDetail(entry);
            await entryDetail.Init();
            return entryDetail;
        }
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public Core.Models.Entry Entry { get; set; }
        public ObservableCollection<EntryName> names = new ObservableCollection<EntryName>();
        public ObservableCollection<EntryName> Names
        {
            get => names;
            set
            {
                SetProperty(ref names, value);
                value.CollectionChanged += (s, e) =>
                {
                    UpdateAlias();
                };
                UpdateAlias();
            }
        }

        private string alias;
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias
        {
            get => alias;
            set => SetProperty(ref alias, value);
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
        private string fullEntryPath;
        public string FullEntryPath
        {
            get => fullEntryPath;
            set => SetProperty(ref fullEntryPath, value);
        }
        private string fullCoverImgPath;
        public string FullCoverImgPath
        {
            get => fullCoverImgPath;
            set => SetProperty(ref fullCoverImgPath, value);
        }
        private string fullMetaDataPath;
        public string FullMetaDataPath
        {
            get => fullMetaDataPath;
            set => SetProperty(ref fullMetaDataPath, value);
        }
        private Core.Models.EntryMetadata metadata;
        public Core.Models.EntryMetadata Metadata
        {
            get => metadata;
            set => SetProperty(ref metadata, value);
        }
        private ObservableCollection<Core.Models.WatchHistory> watchHistory;
        public ObservableCollection<Core.Models.WatchHistory> WatchHistory
        {
            get => watchHistory;
            set => SetProperty(ref watchHistory, value);
        }
        private int watchCount;
        /// <summary>
        /// 有效观看次数
        /// </summary>
        public int WatchCount
        {
            get => watchCount;
            set => SetProperty(ref watchCount, value);
        }
        private List<Core.DbModels.LabelClassDb> labels;
        public List<Core.DbModels.LabelClassDb> Labels
        {
            get => labels;
            set
            {
                SetProperty(ref labels, value);
                Task.Run(() =>
                {
                    Core.Services.LabelService.ClearEntryLabel(Entry.EntryId);//清空词条标签
                    if (value != null)
                    {
                        List<Core.DbModels.EntryLabelClassLinkDb> entryLabelDbs = new List<Core.DbModels.EntryLabelClassLinkDb>(labels.Count);
                        labels.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelClassLinkDb() { EntryId = Entry.EntryId, LCId = p.LCId, DbId = Entry.DbId }));
                        Core.Services.LabelService.AddEntryLabel(entryLabelDbs);//添加词条标签
                    }
                });
            }
        }

        private List<Core.DbModels.LabelPropertyDb> _lpdbs = new List<Core.DbModels.LabelPropertyDb>();
        public List<Core.DbModels.LabelPropertyDb> Lpdbs
        {
            get => _lpdbs;
            set
            {
                SetProperty(ref _lpdbs, value);
                Task.Run(() =>
                {
                    Core.Services.LabelService.ClearEntryLabel(Entry.EntryId);//清空词条标签
                    if (value != null)
                    {
                        List<Core.DbModels.EntryLabelPropertyLinkDb> entryLabelDbs = new List<Core.DbModels.EntryLabelPropertyLinkDb>(_lpdbs.Count);
                        _lpdbs.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelPropertyLinkDb() { EntryId = Entry.EntryId, LPId = p.LPId, DbId = Entry.DbId }));
                        Core.Services.LabelPropertyService.AddEntryLabel(entryLabelDbs);//添加词条标签
                    }
                });
            }
        }
        private ObservableCollection<string> imgs;
        public ObservableCollection<string> Imgs
        {
            get => imgs;
            set => SetProperty(ref imgs, value);
        }
        private async Task Init()
        {
            var namesT = await Core.Services.EntryNameSerivce.QueryNamesAsync(Entry.EntryId, Entry.DbId);
            if (namesT != null)
            {
                Names = namesT.Select(p => new EntryName(p)).ToObservableCollection();
            }
            await UpdateWatchHistoryAsync();
            labels = await Core.Services.LabelService.GetLabelOfEntryAsync(Entry.EntryId);
            Labels ??= new List<Core.DbModels.LabelClassDb>();
            WatchHistory ??= new ObservableCollection<Core.Models.WatchHistory>();



            if (this.Entry.SaveType == SaveType.Folder)
            {
                LoadPathFolder();
            }
            else if ((this.Entry.SaveType == SaveType.Folder))
            {

            }
            else
            {
                LoadLocalRes();
                LoadLocalMetaData();
                LoadLines();
            }

        }
        public async Task UpdateWatchHistoryAsync()
        {
            var histories = (await Core.Services.WatchHistoryService.QueryWatchHistoriesAsync(Entry.EntryId, Entry.DbId)).ToObservableCollection();
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
                        if (Helpers.ImgHelper.IsSupportImg(file.FullName))
                        {
                            Imgs.Add(file.FullName);
                        }
                    }
                }
            }
        }
        public void LoadLocalMetaData()
        {
            Metadata = Core.Models.EntryMetadata.Read(Path.Combine(FullEntryPath, Services.ConfigService.MetadataFileNmae));
        }

        private ObservableCollection<Models.ExplorerItem> videoExplorerItems;
        public ObservableCollection<Models.ExplorerItem> VideoExplorerItems
        {
            get => videoExplorerItems;
            set => SetProperty(ref videoExplorerItems, value);
        }
        public void LoadLocalVideos()
        {
            string folder = Path.Combine(FullEntryPath, Services.ConfigService.VideoFolder);
            if (Directory.Exists(folder))
            {
                VideoExplorerItems = Helpers.FileHelper.FindExplorerItems(folder).FirstOrDefault().Children?.ToObservableCollection();
                if (VideoExplorerItems == null)
                {
                    VideoExplorerItems = new ObservableCollection<ExplorerItem>();
                }
            }
        }

        private ObservableCollection<Models.ExplorerItem> subExplorerItems;
        public ObservableCollection<Models.ExplorerItem> SubExplorerItems
        {
            get => subExplorerItems;
            set => SetProperty(ref subExplorerItems, value);
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

        private ObservableCollection<Models.ExplorerItem> moreExplorerItems;
        public ObservableCollection<Models.ExplorerItem> MoreExplorerItems
        {
            get => moreExplorerItems;
            set => SetProperty(ref moreExplorerItems, value);
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

        private ObservableCollection<Core.Models.ExtractsLineBase> extractsLines;
        public ObservableCollection<Core.Models.ExtractsLineBase> ExtractsLines
        {
            get => extractsLines;
            set => SetProperty(ref extractsLines, value);
        }

        public void LoadLines()
        {
            ExtractsLines = Entry.GetExtractsLines().ToObservableCollection();
            ExtractsLines ??= new ObservableCollection<Core.Models.ExtractsLineBase>();
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

        public string GetVideoFolder()
        {
            return System.IO.Path.Combine(FullEntryPath, Services.ConfigService.VideoFolder);
        }
        public string GetSubFolder()
        {
            return System.IO.Path.Combine(FullEntryPath, Services.ConfigService.SubFolder);
        }
        public string GetResFolder()
        {
            return System.IO.Path.Combine(FullEntryPath, Services.ConfigService.ResourceFolder);
        }

        public string GetImgFolder()
        {
            return System.IO.Path.Combine(FullEntryPath, Services.ConfigService.ImgFolder);
        }

        public bool SaveMetadata()
        {
            return Metadata.Save(System.IO.Path.Combine(FullEntryPath, Services.ConfigService.MetadataFileNmae));
        }

        /// <summary>
        /// 加載指定文件夾
        /// </summary>
        public void LoadPathFolder()
        {
            if (Directory.Exists(PathFolder))
            {
                var items = Helpers.FileHelper.FindExplorerItems(PathFolder).FirstOrDefault().Children?.ToObservableCollection();
                if (items != null && items.Count > 0)
                {
                    VideoExplorerItems = items.Where(
                        a => a.Name.EndsWith(".mp4") ||
                             a.Name.EndsWith(".mkv") ||
                             a.Name.EndsWith(".avi")
                        ).ToObservableCollection();
                    Imgs = items.Where(
                        a => a.Name.EndsWith(".jpg") ||
                             a.Name.EndsWith(".png") ||
                             a.Name.EndsWith(".jpeg")
                    ).Select(a => a.FullName).ToObservableCollection<string>();
                }

                if (VideoExplorerItems == null)
                    VideoExplorerItems = new ObservableCollection<ExplorerItem>();
                if (Imgs == null)
                    Imgs = new ObservableCollection<string>();
            }
        }


        public void LoadLocalRes()
        {
            LoadLocalVideos();
            LoadLocalSubs();
            LoadLocalMore();
            LoadLocalImgs();
        }
    }
}
