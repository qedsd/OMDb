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

namespace OMDb.WinUI3.Services
{
    public static class CommonService
    {
        /// <summary>
        /// 根据地址自动获取封面
        /// </summary>
        /// <returns></returns>
        public static string GetCoverByPath(List<string> lstPath = null, Core.Enums.FileType fileType = Core.Enums.FileType.TotalAll)
        {
            var result = string.Empty;
            if (lstPath == null || !(lstPath.Count > 0))
                result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
            switch (fileType)
            {
                case Core.Enums.FileType.Folder:
                    List<string> filesPath = new List<string>();
                    foreach (var path in lstPath)
                    {
                        //封面空 -> 尋找指定文件夾中圖片 -> 尋找指定文件夾中視頻縮略圖 -> 設置默認封面
                        var files = Helpers.FileHelper.FindExplorerItems(path);
                        var sourceFiles = Helpers.FileHelper.GetAllFiles(files);
                        filesPath.AddRange(sourceFiles.Select(a => a.FullName));
                    }
                    filesPath.Sort(delegate (string str1, string str2)
                    {
                        return GetFileType(str1) - GetFileType(str2);
                    });
                    result = filesPath.Where(a => GetFileType(a).Equals('2')).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = filesPath.Where(a => GetFileType(a).Equals('3')).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(result))
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        else
                        {
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        }
                    }
                    return result;
                case Core.Enums.FileType.Img:
                    return result;
                case Core.Enums.FileType.Video:
                    return result;
                case Core.Enums.FileType.Audio:
                    return result;
                case Core.Enums.FileType.Sub:
                    return result;
                case Core.Enums.FileType.More:
                    return result;
                case Core.Enums.FileType.All:
                    result = lstPath.Where(a => GetFileType(a).Equals('2')).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = lstPath.Where(a => GetFileType(a).Equals('3')).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(result))
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        else
                        {
                            result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                        }
                    }
                    return result;
                case Core.Enums.FileType.TotalAll:
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
        public static char GetFileType(string path)
        {
            var typeAllImg = @"bmp，jpg，png，tif，gif，pcx，tga，exif，fpx，svg，psd，cdr，pcd，dxf，ufo，eps，ai，raw，WMF，webp，avif，apng".ToUpper();
            var typeAllVideo = @"avi、wmv、mpeg、mp4、m4v、mov、asf、flv、f4v、rmvb、rm、3gp、vob".ToUpper();
            var typeAllVideoSub = "Srt、Webvtt、STL、Sbv、Ass、Dfxp、Ttml".ToUpper();
            var typeAllAudio = @"MP3、WAV、WMA、MP2、Flac、MIDI、RA、APE、AAC、CDA、MOV".ToUpper();

            if (path.Contains(@"\"))
            {
                var path_file = path.SubString_A21(@"\", 1, false, false);
                if (path_file.Contains("."))
                {
                    var fileType = path_file.SubString_A21(".", 1, false, false).ToUpper();

                    if (typeAllImg.Contains(fileType))
                        return '2';
                    else if (typeAllVideo.Contains(fileType))
                        return '3';
                    else if (typeAllVideoSub.Contains(fileType))
                        return '4';
                    else if (typeAllAudio.Contains(fileType))
                        return '5';
                    else
                        return '6';
                }
                else
                {
                    return '6';
                }
            }
            else
            {
                if (path.Contains("."))
                {
                    var fileType = path.SubString_A21(".", 1, false, false).ToUpper();

                    if (typeAllImg.Contains(fileType))
                        return '2';
                    else if (typeAllVideo.Contains(fileType))
                        return '3';
                    else if (typeAllVideoSub.Contains(fileType))
                        return '4';
                    else if (typeAllAudio.Contains(fileType))
                        return '5';
                    else
                        return '6';
                }
                else
                {
                    return '6';
                }
            }
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

            //封面空 -> 尋找指定文件夾中圖片 -> 尋找指定文件夾中視頻縮略圖 -> 設置默認封面
            var files = Helpers.FileHelper.FindExplorerItems(path);
            var sourceFiles = Helpers.FileHelper.GetAllFiles(files);
            filesPath.AddRange(sourceFiles.Select(a => a.FullName));

            filesPath.Sort(delegate (string str1, string str2)
            {
                return GetFileType(str1) - GetFileType(str2);
            });
            result = filesPath.Where(a => GetFileType(a).Equals('2')).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(result))
            {
                result = filesPath.Where(a => GetFileType(a).Equals('3')).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(result))
                    result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                else
                {
                    result = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\Img", "DefaultCover.jpg");
                }
            }
            return result;
        }
    }
}

