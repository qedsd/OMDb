using OMDb.Core.Utils;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class FileHelper
    {
        public delegate void ProgressCallBack(float progress);
        /// <summary>
        /// 复制文件
        /// 如果目标文件已存在，则在文件后面加上(数字)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="progressCallBack"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bufferSize"></param>
        /// <returns>返回复制后的文件路径</returns>
        public static string CopyFile(string sourcePath, string targetPath, ProgressCallBack progressCallBack, CancellationToken cancellationToken = default, int bufferSize = 1024 * 1024 * 10)
        {
            byte[] array = new byte[bufferSize]; //创建缓冲区
            using FileStream fsRead = File.Open(sourcePath, FileMode.Open, FileAccess.Read);
            var targetDir = System.IO.Path.GetDirectoryName(targetPath);
            if (!System.IO.Directory.Exists(targetDir))
            {
                System.IO.Directory.CreateDirectory(targetDir);
            }
            if (File.Exists(targetPath))
            {
                int i = 1;
                while (true)
                {
                    string newPath = $"{targetPath}({i++})";
                    if (!Directory.Exists(newPath))
                    {
                        targetPath = newPath;
                        break;
                    }
                }
            }
            using FileStream fsWrite = File.Open(targetPath, FileMode.Create, FileAccess.Write);
            while (fsRead.Position < fsRead.Length)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                //读取到文件缓冲区
                int length = fsRead.Read(array, 0, array.Length);
                //从缓冲区写到新文件
                fsWrite.Write(array, 0, length);
                //计算进度
                var percent = (float)fsRead.Position / fsRead.Length;
                progressCallBack.Invoke(percent);
            }
            return targetPath;
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

        /// <summary>
        /// 获取所有文件，包含文件夹的子文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Models.ExplorerItem> GetImgFiles(string path)
        {
            var items = FindExplorerItems(path);
            return GetAllFiles(items);
        }

        public static void OpenBySystem(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public static int CopyFolder(string sourceFolder, string destFolder, ProgressCallBack progressCallBack, CancellationToken cancellationToken = default)
        {
            try
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    return 0;
                }
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return 0;
                    }
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);//复制文件
                    progressCallBack.Invoke(1);
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return 0;
                    }
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest, progressCallBack,cancellationToken);//构建目标路径,递归复制文件
                }
                return 1;
            }
            catch (Exception e)
            {
                Helpers.InfoHelper.ShowError(e.Message);
                Logger.Error(e);
                return 0;
            }

        }


        /// <summary>
        /// 获取指定路径下所有文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Models.ExplorerItem> FindFolderItems(string path)
        {
            
            List<Models.ExplorerItem> items = new List<Models.ExplorerItem>();
           
            if (Directory.Exists(path))//文件夹
            {
                //根文件夹
                var dire = new DirectoryInfo(path);
                var dirItem = new ExplorerItem()
                {
                    Name = dire.Name,
                    IsFile = false,
                    FullName = dire.FullName,
                };
                items.Add(dirItem);

                //根儿子问加减
                var dirs = new DirectoryInfo(path).GetDirectories();
                if (dirs.Any())
                {
                    dirItem.Children = new List<ExplorerItem>();

                    //递归
                    foreach (var dir in dirs)
                    {
                        dirItem.Children.AddRange(FindFolderItems(dir.FullName));
                    }
                }
                dirItem.Length += dirItem.Children?.Count > 0 ? dirItem.Children.Sum(p => p.Length) : 0;
            }
            return items;
        }



    }
}
