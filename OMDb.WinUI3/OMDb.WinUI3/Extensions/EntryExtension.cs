using OMDb.Core.DbModels;
using OMDb.Core.Extensions;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Extensions
{
    public static class EntryExtension
    {
        public static List<Core.Models.ExtractsLineBase> GetExtractsLines(this Core.Models.Entry entry)
        {
            var metadata = entry.GetMetadata();
            if(metadata != null)
            {
                return metadata.ExtractsLines?.ToList();
            }
            else
            {
                return null;
            }
        }
        public static Core.Models.EntryMetadata GetMetadata(this Core.Models.Entry entry)
        {
            var fullPath = entry.GetFullPath();
            if (!string.IsNullOrEmpty(fullPath))
            {
                return Core.Models.EntryMetadata.Read(Path.Combine(fullPath, Services.ConfigService.MetadataFileNmae));
            }
            else
            {
                return null;
            }
        }
        public static string GetFullPath(this Core.Models.Entry entry)
        {
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (s != null && !string.IsNullOrEmpty(entry.Path))
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(s.StoragePath), entry.Path);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取最佳排序的图片
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="horizontalFirst">true优先横向图片 false优先纵向</param>
        /// <returns></returns>
        public static List<string> GetBestImg(this Core.Models.Entry entry,bool horizontalFirst)
        {
            var fullPath = entry.GetFullPath();
            string imgFolder = Path.Combine(fullPath, Services.ConfigService.ImgFolder);
            if (Directory.Exists(imgFolder))
            {
                var items = Helpers.FileHelper.GetAllFiles(imgFolder);
                List<Core.Models.ImageInfo> infos = new List<Core.Models.ImageInfo>();
                if (items != null && items.Any())
                {
                    foreach (var file in Core.Helpers.RandomHelper.RandomList(items, 100))//仅对100张照片计算
                    {
                        if (Helpers.ImgHelper.IsSupportImg(file.FullName))
                        {
                            infos.Add(Core.Helpers.ImageHelper.GetImageInfo(file.FullName));
                        }
                    }
                }
                //优先匹配长大于宽、文件更大的照片
                List<ImageInfo> sortedInfos;
                if(horizontalFirst)
                {
                    sortedInfos = infos.OrderByDescending(p => p.Scale).OrderByDescending(p => p.Length).ToList();
                }
                else
                {
                    sortedInfos = infos.OrderBy(p => p.Scale).OrderByDescending(p => p.Length).ToList();
                }
                int[] weights = new int[sortedInfos.Count];
                for (int i = 0; i < sortedInfos.Count; i++)
                {
                    weights[i] = i + 1;//权重从1开始递增
                }
                var coverItems = Core.Helpers.RandomHelper.RandomList(sortedInfos, weights, 1);//获取最优的
                return coverItems?.Select(p => p.FullPath).ToList();
            }
            else
            {
                return null;
            }
        }
    }
}
