using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Core;
using NPOI.POIFS.FileSystem;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Helpers;
using OMDb.Core.Models;
using OMDb.Core.Services;
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

namespace OMDb.WinUI3.Models
{
    public partial class EntryDetail : ObservableObject
    {
        public Core.Models.Entry Entry { get; set; }

        #region 文件夹存储->文件夹路径
        private string _pathFolder = string.Empty;
        public string PathFolder
        {
            get => _pathFolder;
            set => SetProperty(ref _pathFolder, value);
        }
        #endregion

        #region 图片路径集合
        private ObservableCollection<string> _pathImg = new ObservableCollection<string>();
        public ObservableCollection<string> PathImg
        {
            get => _pathImg;
            set => SetProperty(ref _pathImg, value);
        }
        #endregion

        #region 视频路径集合
        private ObservableCollection<string> _pathVideo = new ObservableCollection<string>();
        public ObservableCollection<string> PathVideo
        {
            get => _pathVideo;
            set => SetProperty(ref _pathVideo, value);
        }
        #endregion

        #region 音频路径集合
        private ObservableCollection<string> _pathAudio = new ObservableCollection<string>();
        public ObservableCollection<string> PathAudio
        {
            get => _pathAudio;
            set => SetProperty(ref _pathAudio, value);
        }
        #endregion

        #region 更多文件路径集合
        private ObservableCollection<string> _pathMore = new ObservableCollection<string>();
        public ObservableCollection<string> PathMore
        {
            get => _pathMore;
            set => SetProperty(ref _pathMore, value);
        }
        #endregion

        #region 词条名称(默认)
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        #endregion

        #region 词条名称集合
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
        #endregion

        #region 词条别名
        private string alias;
        public string Alias
        {
            get => alias;
            set => SetProperty(ref alias, value);
        }
        #endregion

        #region 词条路径
        private string fullEntryPath;
        public string FullEntryPath
        {
            get => fullEntryPath;
            set => SetProperty(ref fullEntryPath, value);
        }
        #endregion

        #region 封面路径
        private string fullCoverImgPath;
        public string FullCoverImgPath
        {
            get => fullCoverImgPath;
            set => SetProperty(ref fullCoverImgPath, value);
        }
        #endregion

        #region 元数据路径
        private string fullMetaDataPath;
        public string FullMetaDataPath
        {
            get => fullMetaDataPath;
            set => SetProperty(ref fullMetaDataPath, value);
        }

        #endregion

        #region 元数据
        private Core.Models.EntryMetadata metadata;
        public Core.Models.EntryMetadata Metadata
        {
            get => metadata;
            set => SetProperty(ref metadata, value);
        }
        #endregion

        #region 观看历史
        private ObservableCollection<Core.Models.WatchHistory> watchHistory;
        public ObservableCollection<Core.Models.WatchHistory> WatchHistory
        {
            get => watchHistory;
            set => SetProperty(ref watchHistory, value);
        }
        #endregion

        #region 观看次数
        private int watchCount;
        public int WatchCount
        {
            get => watchCount;
            set => SetProperty(ref watchCount, value);
        }
        #endregion

        #region 标签列表(分类)
        private List<Core.DbModels.LabelClassDb> _labelClassDbList;
        public List<Core.DbModels.LabelClassDb> LabelClassDbList
        {
            get => _labelClassDbList;
            set
            {
                SetProperty(ref _labelClassDbList, value);
                Task.Run(() =>
                {
                    Core.Services.LabelClassService.ClearEntryLabel(Entry.EntryId);//清空词条标签
                    if (value != null)
                    {
                        List<Core.DbModels.EntryLabelClassLinkDb> entryLabelDbs = new List<Core.DbModels.EntryLabelClassLinkDb>(_labelClassDbList.Count);
                        _labelClassDbList.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelClassLinkDb() { EntryId = Entry.EntryId, LCID = p.LCID, DbId = Entry.DbId }));
                        Core.Services.LabelClassService.AddEntryLabel(entryLabelDbs);//添加词条标签
                    }
                });
            }
        }
        #endregion

        #region 标签列表(属性)
        private List<Core.DbModels.LabelPropertyDb> _lablePropertyDbList = new List<Core.DbModels.LabelPropertyDb>();
        public List<Core.DbModels.LabelPropertyDb> LablePropertyDbList
        {
            get => _lablePropertyDbList;
            set
            {
                SetProperty(ref _lablePropertyDbList, value);
                Task.Run(() =>
                {
                    Core.Services.LabelClassService.ClearEntryLabel(Entry.EntryId);//清空词条标签
                    if (value != null)
                    {
                        List<Core.DbModels.EntryLabelPropertyLinkDb> entryLabelDbs = new List<Core.DbModels.EntryLabelPropertyLinkDb>(_lablePropertyDbList.Count);
                        _lablePropertyDbList.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelPropertyLinkDb() { EntryId = Entry.EntryId, LPID = p.LPID, DbId = Entry.DbId }));
                        Core.Services.LabelPropertyService.AddEntryLabel(entryLabelDbs);//添加词条标签
                    }
                });
            }
        }
        #endregion

        #region 图片文件
        private ObservableCollection<string> imgs;
        public ObservableCollection<string> Imgs
        {
            get => imgs;
            set => SetProperty(ref imgs, value);
        }
        #endregion

        #region 视频文件
        private ObservableCollection<Models.ExplorerItem> videoExplorerItems;
        public ObservableCollection<Models.ExplorerItem> VideoExplorerItems
        {
            get => videoExplorerItems;
            set => SetProperty(ref videoExplorerItems, value);
        }
        #endregion

        #region 字幕文件
        private ObservableCollection<Models.ExplorerItem> subExplorerItems;
        public ObservableCollection<Models.ExplorerItem> SubExplorerItems
        {
            get => subExplorerItems;
            set => SetProperty(ref subExplorerItems, value);
        }
        #endregion

        #region 更多文件
        private ObservableCollection<Models.ExplorerItem> moreExplorerItems;
        public ObservableCollection<Models.ExplorerItem> MoreExplorerItems
        {
            get => moreExplorerItems;
            set => SetProperty(ref moreExplorerItems, value);
        }
        #endregion

        #region 台词摘录
        private ObservableCollection<Core.Models.ExtractsLineBase> extractsLines;
        public ObservableCollection<Core.Models.ExtractsLineBase> ExtractsLines
        {
            get => extractsLines;
            set => SetProperty(ref extractsLines, value);
        }
        #endregion

    }
}
