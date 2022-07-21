using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class FileHelper
    {
        //public static void CopyAllFiles(string sourcePath,string targetPath)
        //{
        //    var items = GetAllFiles(sourcePath);
        //    if (items?.Count!=0)
        //    {
        //        foreach(var item in items)
        //        {
        //            File.Copy(item.FullName, Path.Combine(targetPath,item.Name), true);
        //        }
        //    }
        //}
        public delegate void ProgressCallBack(float progress);
        public static void CopyFiles(string sourcePath, string targetPath, ProgressCallBack progressCallBack, int bufferSize = 1024 * 1024)
        {
            byte[] array = new byte[bufferSize]; //创建缓冲区
            using FileStream fsRead = File.Open(sourcePath, FileMode.Open, FileAccess.Read);
            using FileStream fsWrite = File.Open(targetPath, FileMode.Create, FileAccess.Write);
            while (fsRead.Position < fsRead.Length)
            {
                //读取到文件缓冲区
                int length = fsRead.Read(array, 0, array.Length);
                //从缓冲区写到新文件
                fsWrite.Write(array, 0, length);
                //计算进度
                var percent = (float)fsRead.Position / fsRead.Length;
                progressCallBack.Invoke(percent);
            }
        }

        /// <summary>
        /// 获取指定路径下所有文件、文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Models.ExplorerItem> FindExplorerItems(string path)
        {
            List<Models.ExplorerItem> items = new List<Models.ExplorerItem>();
            if (Path.HasExtension(path) && File.Exists(path))//文件
            {
                FileInfo fileInfo = new FileInfo(path);
                items.Add(new Models.ExplorerItem()
                {
                    Name = fileInfo.Name,
                    IsFile = true,
                    Length = fileInfo.Length,
                    FullName = fileInfo.FullName,
                });
            }
            else if (Directory.Exists(path))//文件夹
            {
                var dire = new DirectoryInfo(path);
                var dirItem = new ExplorerItem()
                {
                    Name = dire.Name,
                    IsFile = false,
                    FullName = dire.FullName,
                };
                items.Add(dirItem);
                var dirs = new DirectoryInfo(path).GetDirectories();
                if (dirs.Any())
                {
                    dirItem.Children = new List<ExplorerItem>();

                    foreach (var dir in dirs)
                    {
                        dirItem.Children.AddRange(FindExplorerItems(dir.FullName));
                    }
                }
                var files = new DirectoryInfo(path).GetFiles();
                if (files.Any())
                {
                    if (dirItem.Children == null)
                    {
                        dirItem.Children = new List<ExplorerItem>();
                    }
                    foreach (var file in files)
                    {
                        dirItem.Children.AddRange(FindExplorerItems(file.FullName));
                    }
                }
                dirItem.Length += dirItem.Children?.Count > 0 ? dirItem.Children.Sum(p => p.Length) : 0;
            }
            return items;
        }

        /// <summary>
        /// 获取所有文件，包含文件夹的子文件
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<Models.ExplorerItem> GetAllFiles(IEnumerable<Models.ExplorerItem> items)
        {
            if (items == null)
            {
                return null;
            }
            else
            {
                List<Models.ExplorerItem> files = new List<ExplorerItem>();
                foreach (var item in items)
                {
                    if (item.IsFile)
                    {
                        files.Add(item);
                    }
                    else if (item.Children?.Count > 0)
                    {
                        files.AddRange(GetAllFiles(item.Children));
                    }
                }
                return files;
            }
        }
        /// <summary>
        /// 获取所有文件，包含文件夹的子文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Models.ExplorerItem> GetAllFiles(string path)
        {
            var items = FindExplorerItems(path);
            return GetAllFiles(items);
        }

        
    }
}
