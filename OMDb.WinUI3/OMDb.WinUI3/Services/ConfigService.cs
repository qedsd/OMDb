using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string DefaultEntryFolder 
        { 
            get;
            set;
        }  


        /// <summary>
        /// 资源文件夹
        /// </summary>
        public static string ResourceFolder { get; } = @"Resource";
        /// <summary>
        /// 资源文件夹->图片资源文件夹
        /// </summary>
        public static string ImgFolder { get; } = @"Resource\Img";
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
        public static string InfoFolder { get; } = "Info";


        /// <summary>
        /// 元文件文件名
        /// </summary>
        public static string MetadataFileNmae { get; } = "metadata.json";

        public static ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; }
        private static string EnrtyStorageFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "storages.json");
        public static void Load()
        {    
            Core.Config.InitLocalDb(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "db.db"));
            Core.Config.SetFFmpegExecutablesPath(System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "FFmpeg"));
            LoadStorages();
        }
        public static async void LoadStorages()
        {
            EnrtyStorages = new ObservableCollection<EnrtyStorage>();
            Core.Config.ClearDb();
            var lstStorage=await Core.Services.StorageService.GetAllStorageAsync();
            if (EnrtyStorages != null) { EnrtyStorages.Clear(); } else { EnrtyStorages = new ObservableCollection<EnrtyStorage>(); }
            if (lstStorage != null)
            {
                foreach (var item in lstStorage)
                {
                    EnrtyStorage enrtyStorage = new EnrtyStorage();
                    enrtyStorage.StorageName = item.StorageName;
                    enrtyStorage.StoragePath = item.StoragePath;
                    enrtyStorage.CoverImg = item.CoverImg;
                    enrtyStorage.EntryCount = (int)item.EntryCount;
                    EnrtyStorages.Add(enrtyStorage);

                    Core.Config.AddDbFile(item.StoragePath, item.StorageName, false);

                }

            }
        }
        public static void Save()
        {
            string json = JsonConvert.SerializeObject(EnrtyStorages.Take(EnrtyStorages.Count - 1));
            string entryStoragePath = System.IO.Path.GetDirectoryName(EnrtyStorageFile);
            if (!System.IO.Directory.Exists(entryStoragePath))
            {
                System.IO.Directory.CreateDirectory(entryStoragePath);
            }
            System.IO.File.WriteAllText(EnrtyStorageFile, json);
        }

        
    }
}
