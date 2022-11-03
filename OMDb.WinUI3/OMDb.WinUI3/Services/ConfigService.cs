using Newtonsoft.Json;
using OMDb.WinUI3.Models;
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
        public static string DefaultEntryFolder { get; } = "Entries";
        /// <summary>
        /// 字幕文件夹
        /// </summary>
        public static string SubFolder { get; } = "Sub";
        /// <summary>
        /// 图片文件夹
        /// </summary>
        public static string ImgFolder { get; } = "Img";
        /// <summary>
        /// 视频文件夹
        /// </summary>
        public static string VideoFolder { get; } = "Video";
        /// <summary>
        /// 下载源文件夹
        /// BT之类
        /// </summary>
        public static string ResourceFolder { get; } = "Resource";
        /// <summary>
        /// 元文件文件名
        /// </summary>
        public static string MetadataFileNmae { get; } = "metadata.json";

        public static ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; } = new ObservableCollection<EnrtyStorage>();
        private static string EnrtyStorageFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "storages.json");
        public static void Load()
        {
            if (System.IO.File.Exists(EnrtyStorageFile))
            {
                string json = System.IO.File.ReadAllText(EnrtyStorageFile);
                var items = JsonConvert.DeserializeObject<List<EnrtyStorage>>(json);
                if (items != null)
                {
                    items.ForEach(p =>
                    {
                        EnrtyStorages.Add(p);
                        Core.Config.AddDbFile(p.StoragePath, p.StorageName, false);
                    });
                }
            }
            Core.Config.InitLocalDb(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "db.db"));
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
