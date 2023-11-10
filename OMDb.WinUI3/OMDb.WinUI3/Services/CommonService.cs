using OMDb.Core.Utils;
using NPOI.OpenXmlFormats.Vml;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMDb.Core.Utils.StringUtil;
using OMDb.Core.Services;
using OMDb.Core.Enums;
using OMDb.Core.Utils.Extensions;
using OMDb.Core.Const;
using OMDb.Core.Models;
using OMDb.WinUI3.Extensions;
using OMDb.Core.Helpers;
using OMDb.WinUI3.Services.Settings;
using System.Collections.ObjectModel;

namespace OMDb.WinUI3.Services
{
    public static class CommonService
    {
        /// <summary>
        /// 根据地址自动获取封面
        /// </summary>
        /// <returns></returns>
        public static string GetCover(string path = null)
        {
            var result = string.Empty;

            result = ConfigService.DefaultCover;
            if (Directory.Exists(path))
            {
                var files = Helpers.FileHelper.FindExplorerItems(path);
                var sourceFiles = Helpers.FileHelper.GetAllFiles(files);
                var filesPath = sourceFiles.Select(a => a.FullName);
                var imgs = filesPath.Where(x => GetPathType(x) == PathType.Image);
                if (!imgs.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                    result = ImageHelper.GetBestImg(imgs, Const.Scale_Cover).FirstOrDefault();
            }
            return result;
        }

        /// <summary>
        /// 根据地址自动获取封面
        /// </summary>
        /// <returns></returns>
        public static string GetCover(List<string> paths)
        {
            var result = string.Empty;

            result = ConfigService.DefaultCover;

            if (paths.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                result = ImageHelper.GetBestImg(paths, Const.Scale_Cover).FirstOrDefault();

            return result;
        }





        /// <summary>
        /// 根据文件名获取文件类型(数据库版本) 图片->2 / 视频->3 / 音频->4 / 字幕->5 / 其他->6
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns> 
        public static PathType GetPathType(string path)
        {
            var isExist = File.Exists(path);
            if (!isExist)
                return PathType.More;

            var file_extension = Path.GetExtension(path).Remove(".").ToUpper();

            if (MediaTypes.Image.Contains(file_extension))
                return PathType.Image;

            if (MediaTypes.Video.Contains(file_extension))
                return PathType.Video;

            if (MediaTypes.VideoSub.Contains(file_extension))
                return PathType.VideoSub;

            if (MediaTypes.Audio.Contains(file_extension))
                return PathType.Audio;

            return PathType.More;
        }





        public static async Task<List<LabelClassTree>> GetLabelClassTrees()
        {
            var labels = await Core.Services.LabelClassService.GetAllLabelAsync(DbSelectorService.dbCurrentId);
            var labelTrees = new List<LabelClassTree>();
            if (labels != null)
            {
                Dictionary<string, LabelClassTree> labelsDb = new Dictionary<string, LabelClassTree>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if (root != null)
                {
                    foreach (var label in root)
                    {
                        labelsDb.Add(label.LCID, new LabelClassTree(label));
                    }
                }
                foreach (var label in labels)
                {
                    if (label.ParentId != null)
                    {
                        if (labelsDb.TryGetValue(label.ParentId, out var parent))
                        {
                            parent.Children.Add(new LabelClassTree(label));
                        }
                    }
                }
                foreach (var item in labelsDb)
                {
                    labelTrees.Add(item.Value);
                }
            }
            return labelTrees;
        }

        public static async Task<List<LabelPropertyTree>> GetLabelPropertyTrees()
        {
            var labels = await Core.Services.LabelPropertyService.GetAllLabelPropertyAsync(DbSelectorService.dbCurrentId);
            var labelTrees = new List<LabelPropertyTree>();
            if (labels != null)
            {
                Dictionary<string, LabelPropertyTree> labelsDb = new Dictionary<string, LabelPropertyTree>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if (root != null)
                {
                    foreach (var label in root)
                    {
                        labelsDb.Add(label.LPID, new LabelPropertyTree(label));
                    }
                }
                foreach (var label in labels)
                {
                    if (label.ParentId != null)
                    {
                        if (labelsDb.TryGetValue(label.ParentId, out var parent))
                        {
                            parent.Children.Add(new LabelPropertyTree(label));
                        }
                    }
                }
                foreach (var item in labelsDb)
                {
                    labelTrees.Add(item.Value);
                }
            }
            return labelTrees;
        }




    }
}

