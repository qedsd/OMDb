using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OMDb.Maui.Helpers
{
    /// <summary>
    /// 文件操作助手 - 提供文件复制、文件夹操作、文件浏览等功能
    ///
    /// 主要功能：
    /// 1. CopyFileAsync - 复制文件（支持进度回调和取消）
    /// 2. CopyFolderAsync - 复制文件夹及子文件夹（支持进度回调和取消）
    /// 3. OpenBySystem - 使用系统默认方式打开文件/文件夹
    /// 4. GetAllFiles - 获取指定路径下所有文件（递归）
    /// 5. FindExplorerItems - 获取指定路径下的文件和文件夹
    ///
    /// 注意：MAUI 版本移除了 ExplorerItem 相关方法，简化为纯文件操作
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 进度回调委托
        /// </summary>
        /// <param name="progress">进度值（0-1 之间）</param>
        public delegate void ProgressCallBack(float progress);

        /// <summary>
        /// 复制文件
        /// 如果目标文件已存在，则在文件后面加上 (数字)
        ///
        /// 使用示例：
        /// <code>
        /// string resultPath = FileHelper.CopyFile(
        ///     sourcePath: "C:\source\file.txt",
        ///     targetPath: "C:\dest\file.txt",
        ///     progressCallBack: p => Console.WriteLine($"进度：{p:P}"),
        ///     cancellationToken: CancellationToken.None
        /// );
        /// </code>
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="progressCallBack">进度回调委托</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="bufferSize">缓冲区大小（默认 10MB）</param>
        /// <returns>复制后的文件路径</returns>
        public static string CopyFile(string sourcePath, string targetPath, ProgressCallBack progressCallBack, CancellationToken cancellationToken = default, int bufferSize = 1024 * 1024 * 10)
        {
            byte[] array = new byte[bufferSize]; // 创建缓冲区
            using FileStream fsRead = File.Open(sourcePath, FileMode.Open, FileAccess.Read);
            var targetDir = System.IO.Path.GetDirectoryName(targetPath);

            // 如果目标目录不存在，则创建
            if (!System.IO.Directory.Exists(targetDir))
            {
                System.IO.Directory.CreateDirectory(targetDir);
            }

            // 如果目标文件已存在，生成新文件名（添加序号）
            if (File.Exists(targetPath))
            {
                int i = 1;
                while (true)
                {
                    string newPath = $"{targetPath}({i++})";
                    if (!File.Exists(newPath))
                    {
                        targetPath = newPath;
                        break;
                    }
                }
            }

            using FileStream fsWrite = File.Open(targetPath, FileMode.Create, FileAccess.Write);

            // 读取源文件并写入目标文件
            while (fsRead.Position < fsRead.Length)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // 读取到文件缓冲区
                int length = fsRead.Read(array, 0, array.Length);
                // 从缓冲区写入新文件
                fsWrite.Write(array, 0, length);
                // 计算进度
                var percent = (float)fsRead.Position / fsRead.Length;
                progressCallBack?.Invoke(percent);
            }

            return targetPath;
        }

        /// <summary>
        /// 复制文件夹及子文件夹（异步版本）
        ///
        /// 使用示例：
        /// <code>
        /// var cts = new CancellationTokenSource();
        /// int result = await FileHelper.CopyFolderAsync(
        ///     sourceFolder: "C:\source",
        ///     destFolder: "C:\dest",
        ///     progressCallBack: p => Console.WriteLine($"进度：{p:P}"),
        ///     cancellationToken: cts.Token
        /// );
        /// </code>
        /// </summary>
        /// <param name="sourceFolder">源文件夹路径</param>
        /// <param name="destFolder">目标文件夹路径</param>
        /// <param name="progressCallBack">进度回调委托</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>1=成功，0=失败或取消</returns>
        public static Task<int> CopyFolderAsync(string sourceFolder, string destFolder, ProgressCallBack progressCallBack, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => CopyFolder(sourceFolder, destFolder, progressCallBack, cancellationToken));
        }

        /// <summary>
        /// 复制文件夹及子文件夹（同步版本）
        ///
        /// 递归复制源文件夹下的所有文件和子文件夹到目标位置
        /// 如果目标路径不存在，会自动创建
        ///
        /// 注意：此方法不处理文件已存在的情况，会直接覆盖
        /// </summary>
        /// <param name="sourceFolder">源文件夹路径</param>
        /// <param name="destFolder">目标文件夹路径</param>
        /// <param name="progressCallBack">进度回调委托</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>1=成功，0=失败或取消</returns>
        public static int CopyFolder(string sourceFolder, string destFolder, ProgressCallBack progressCallBack, CancellationToken cancellationToken = default)
        {
            try
            {
                // 检查取消请求
                if (cancellationToken.IsCancellationRequested)
                {
                    return 0;
                }

                // 如果目标路径不存在，则创建
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }

                // 得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return 0;
                    }
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest, true); // true=覆盖已存在的文件
                    progressCallBack?.Invoke(1);
                }

                // 得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return 0;
                    }
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    // 递归复制子文件夹
                    CopyFolder(folder, dest, progressCallBack, cancellationToken);
                }

                return 1;
            }
            catch (Exception e)
            {
                // 记录错误（使用 InfoHelper 显示错误）
                _ = InfoHelper.ShowErrorAsync($"复制文件夹出错：{e.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 使用系统默认方式打开文件
        ///
        /// 在 Windows 上：使用资源管理器打开
        /// 在 macOS 上：使用 Finder 打开
        /// 在 Linux 上：使用文件管理器打开
        ///
        /// 使用示例：
        /// <code>
        /// FileHelper.OpenBySystem("C:\Videos\movie.mp4"); // 打开文件所在文件夹并选中文件
        /// FileHelper.OpenBySystem("C:\Videos\"); // 打开文件夹
        /// </code>
        /// </summary>
        /// <param name="path">文件或文件夹路径</param>
        public static void OpenBySystem(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path) && !System.IO.Directory.Exists(path))
            {
                InfoHelper.ShowErrorAsync("无效的文件路径");
                return;
            }

            try
            {
#if WINDOWS
                // Windows: 使用 explorer.exe 打开
                System.Diagnostics.Process.Start("explorer.exe", path);
#elif __MACOS__
                // macOS: 使用 open 命令
                var proc = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = $"\"{path}\"",
                        UseShellExecute = true
                    }
                };
                proc.Start();
#elif __LINUX__
                // Linux: 使用 xdg-open 打开
                var proc = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = $"\"{path}\"",
                        UseShellExecute = true
                    }
                };
                proc.Start();
#else
                // 其他平台：尝试使用 Shell 打开
                System.Diagnostics.Process.Start(path);
#endif
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"打开文件失败：{ex.Message}");
            }
        }
    }
}
