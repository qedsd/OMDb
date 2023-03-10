using OMDb.WinUI3.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class PathService
    {
        /// <summary>
        /// 词条存储路径文件夹相对于仓库路径
        /// 需要此时的词条存储路径为全路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string EntryRelativePath(Core.Models.Entry entry)
        {
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (s != null && !string.IsNullOrEmpty(entry.Path))
            {
                return entry.Path.Replace(s.StoragePath, string.Empty);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 词条封面图完整路径
        /// 需要此时的词条封面图路径为相对路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string EntryCoverImgFullPath(Core.Models.Entry entry)
        {
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (s != null && !string.IsNullOrEmpty(entry.Path))
            {
                return System.IO.Path.Combine(s.StoragePath + entry.Path, entry.CoverImg);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 词条存储路径文件夹完整路径
        /// 需要此时的词条存储路径为相对路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string EntryFullPath(Core.Models.Entry entry)
        {
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (s != null && !string.IsNullOrEmpty(entry.Path))
            {
                return System.IO.Path.Combine(s.StoragePath, ConfigService.DefaultEntryFolder, entry.Path);
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 词条封面图相对于仓库路径
        /// 需要此时的词条存储路径、词条封面图路径都为全路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string EntryCoverImgRelativePath(Core.Models.Entry entry)
        {
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (s != null && !string.IsNullOrEmpty(entry.Path))
            {
                return entry.CoverImg.Substring(entry.Path.Length + 1);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 根据词条名补充词条完整路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string GetFullEntryPathByEntryName(string entryName,string storePath)
        {
            return Path.Combine(storePath,ConfigService.DefaultEntryFolder, entryName);
        }
    }
}
