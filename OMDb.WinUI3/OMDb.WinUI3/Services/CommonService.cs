using OMDb.Core.Utils;
using NPOI.OpenXmlFormats.Vml;
using OMDb.Core.Extensions;
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

namespace OMDb.WinUI3.Services
{
    public static class CommonService
    {
        /// <summary>
        /// 根据地址自动获取封面
        /// </summary>
        /// <returns></returns>
        public static string GetCoverByPath(List<string> lstPath = null, Core.Enums.PathType fileType = Core.Enums.PathType.TotalAll)
        {
            var result = string.Empty;
            if (lstPath == null || !(lstPath.Count > 0))
                result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
            switch (fileType)
            {
                case Core.Enums.PathType.Folder:
                    List<string> filesPath = new List<string>();
                    foreach (var path in lstPath)
                    {
                        //封面空 -> 尋找指定文件夾中圖片 -> 尋找指定文件夾中視頻縮略圖 -> 設置默認封面
                        var files = Helpers.FileHelper.FindExplorerItems(path);
                        var sourceFiles = Helpers.FileHelper.GetAllFiles(files);
                        filesPath.AddRange(sourceFiles.Select(a => a.FullName));
                    }

                    result = filesPath.Where(a => GetPathType(a).Equals('2')).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = filesPath.Where(a => GetPathType(a).Equals('3')).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(result))
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        else
                        {
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        }
                    }
                    return result;
                case Core.Enums.PathType.Image:
                    return result;
                case Core.Enums.PathType.Video:
                    return result;
                case Core.Enums.PathType.Audio:
                    return result;
                case Core.Enums.PathType.VideoSub:
                    return result;
                case Core.Enums.PathType.More:
                    return result;
                case Core.Enums.PathType.All:
                    result = lstPath.Where(a => GetPathType(a).Equals('2')).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = lstPath.Where(a => GetPathType(a).Equals('3')).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(result))
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        else
                        {
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        }
                    }
                    return result;
                case Core.Enums.PathType.TotalAll:
                    return result;
                default:
                    return result;

            }

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
            var typeAllImg = @"BMP，JPG，PNG，TIF，GIF，PCX，TGA，EXIF，FPX，SVG，PSD，CDR，PCD，DXF，UFO，EPS，AI，RAW，WMF，WEBP，AVIF，APNG";
            var typeAllVideo = @"AVI、WMV、MPEG、MP4、M4V、MOV、ASF、FLV、F4V、RMVB、RM、3GP、VOB";
            var typeAllVideoSub = @"SRT、WEBVTT、STL、SBV、ASS、DFXP、TTML";
            var typeAllAudio = @"MP3、WAV、WMA、MP2、Flac、MIDI、RA、APE、AAC、CDA、MOV";
            var file_extension = Path.GetExtension(path).Delete1stChar().ToUpper();
            if (typeAllImg.Contains(file_extension))
                return PathType.Image;
            else if (typeAllVideo.Contains(file_extension))
                return PathType.Video;
            else if (typeAllVideoSub.Contains(file_extension))
                return PathType.VideoSub;
            else if (typeAllAudio.Contains(file_extension))
                return PathType.Audio;
            else
                return PathType.More;
        }


        /// <summary>
        /// 根据地址自动获取封面
        /// </summary>
        /// <returns></returns>
        public static string GetCoverByPath(string path)
        {
            var result = string.Empty;
            if (!path.NotNullAndEmpty())
                result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
            List<string> filesPath = new List<string>();

            //封面空 -> 尋找指定文件夾中圖片 -> 設置默認封面
            var files = Helpers.FileHelper.FindExplorerItems(path);
            var sourceFiles = Helpers.FileHelper.GetAllFiles(files);
            filesPath.AddRange(sourceFiles.Select(a => a.FullName));

            result = filesPath.Where(a => GetPathType(a).Equals("Image")).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(result))
                result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
            return result;
        }
    }
}

