using Microsoft.UI.Xaml.Shapes;
using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services.Settings;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class ConfigService
    {
        /// <summary>
        /// 默认词条路径
        /// 相对于仓库路径
        /// </summary>
        public static string DefaultEntryFolder { get; set; }
        /// <summary>
        /// 资源文件夹
        /// </summary>
        public static string ResourceFolder { get; } = @"Resource";
        /// <summary>
        /// 资源文件夹->图片资源文件夹
        /// </summary>
        public static string ImgFolder { get; } = @"Resource\Image";
        /// <summary>
        /// 资源文件夹->视频资源文件夹
        /// </summary>
        public static string AudioFolder { get; } = @"Resource\Audio";
        /// <summary>
        /// 资源文件夹->视频资源文件夹
        /// </summary>
        public static string VideoFolder { get; } = @"Resource\Video";
        /// <summary>
        /// 资源文件夹->视频资源文件夹->字幕资源文件夹
        /// </summary>
        public static string SubFolder { get; } = @"Resource\Video\Sub";
        /// <summary>
        /// 资源文件夹->视频资源文件夹
        /// </summary>
        public static string MoreFolder { get; } = @"Resource\More";

        public static string InfoFolder { get; } = "Info";
        public static string OMDbFolder { get; } = ".omdb";
        public static string StorageDbName { get; } = "omdb.db";
        public static string DefaultCover { get; private set; } = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
        /// <summary>
        /// 元文件文件名
        /// </summary>
        public static string MetadataFileNmae { get; } = "MetaData.json";

        public static ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; }

        public static void Load()
        {
            Core.Config.InitMCDb(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "MCDb.db"));
            SettingService.Load();

            Core.Config.SetFFmpegExecutablesPath(System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "FFmpeg"));
            LoadStorages();
        }
        public static async void LoadStorages()
        {
            EnrtyStorages = new ObservableCollection<EnrtyStorage>();
            Core.Config.ClearDb();
            var lstStorage = await Core.Services.StorageService.GetAllStorageAsync(Services.Settings.DbSelectorService.dbCurrentId);

            //初始化 EnrtyStorages
            if (EnrtyStorages != null)
                EnrtyStorages.Clear();
            else
                EnrtyStorages = new ObservableCollection<EnrtyStorage>();

            if (lstStorage != null)
            {
                foreach (var item in lstStorage)
                {
                    EnrtyStorage enrtyStorage = new EnrtyStorage();
                    enrtyStorage.StorageId = item.Id;
                    enrtyStorage.StorageName = item.StorageName;
                    enrtyStorage.StoragePath = item.StoragePath;
                    enrtyStorage.CoverImg = item.CoverImg;
                    enrtyStorage.EntryCount = (int)item.EntryCount;
                    EnrtyStorages.Add(enrtyStorage);
                    var path_db = System.IO.Path.Combine(item.StoragePath, OMDbFolder, Services.Settings.DbSelectorService.dbCurrentName, StorageDbName);
                    Core.Config.AddDbFile(path_db, item.StorageName, false);
                }

            }
        }
    }
}
