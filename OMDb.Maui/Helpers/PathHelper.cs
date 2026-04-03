using System;
using System.IO;
using System.Threading.Tasks;

namespace OMDb.Maui.Helpers
{
    /// <summary>
    /// 路径操作助手 - 提供路径相关的辅助功能
    ///
    /// 主要功能：
    /// 1. OpenBySystem - 使用系统默认方式打开文件所在文件夹
    /// 2. GetFolderPath - 获取文件夹路径（确保以目录分隔符结尾）
    /// 3. GetFileName - 从完整路径中提取文件名
    /// 4. GetDirectoryName - 从完整路径中提取目录名
    /// 5. CombinePath - 安全地组合多个路径
    ///
    /// 注意：MAUI 版本移除了 CalFolderSize 方法（依赖 Windows cmd），
    /// 可使用跨平台的 GetDirectorySizeAsync 替代
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// 使用系统默认方式打开文件所在文件夹
        /// 与 FileHelper.OpenBySystem 不同，此方法总是打开文件夹而不是文件
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.OpenBySystem("C:\Videos\movie.mp4"); // 打开 C:\Videos\ 文件夹
        /// </code>
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void OpenBySystem(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                InfoHelper.ShowErrorAsync("路径不能为空");
                return;
            }

            try
            {
                string directory = Path.GetDirectoryName(path);
                if (Directory.Exists(directory))
                {
#if WINDOWS
                    System.Diagnostics.Process.Start("explorer.exe", directory);
#elif __MACOS__
                    var proc = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "open",
                            Arguments = $"\"{directory}\"",
                            UseShellExecute = true
                        }
                    };
                    proc.Start();
#elif __LINUX__
                    var proc = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "xdg-open",
                            Arguments = $"\"{directory}\"",
                            UseShellExecute = true
                        }
                    };
                    proc.Start();
#else
                    System.Diagnostics.Process.Start(directory);
#endif
                }
                else
                {
                    InfoHelper.ShowErrorAsync($"文件夹不存在：{directory}");
                }
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"打开文件夹失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取文件夹路径（确保以目录分隔符结尾）
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.GetFolderPath("C:\Videos") // 返回 "C:\Videos\" (Windows)
        /// PathHelper.GetFolderPath("/home/user/videos") // 返回 "/home/user/videos/" (Linux/Mac)
        /// </code>
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>以目录分隔符结尾的路径</returns>
        public static string GetFolderPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // 如果路径已经是目录分隔符结尾，直接返回
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path;

            // 添加目录分隔符
            return path + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// 从完整路径中提取文件名（包含扩展名）
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.GetFileName("C:\Videos\movie.mp4") // 返回 "movie.mp4"
        /// PathHelper.GetFileName("/home/user/doc.pdf") // 返回 "doc.pdf"
        /// </code>
        /// </summary>
        /// <param name="path">完整路径</param>
        /// <returns>文件名（包含扩展名）</returns>
        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return Path.GetFileName(path);
        }

        /// <summary>
        /// 从完整路径中提取文件名（不包含扩展名）
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.GetFileNameWithoutExtension("C:\Videos\movie.mp4") // 返回 "movie"
        /// </code>
        /// </summary>
        /// <param name="path">完整路径</param>
        /// <returns>文件名（不包含扩展名）</returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// 从完整路径中提取目录名
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.GetDirectoryName("C:\Videos\movie.mp4") // 返回 "C:\Videos"
        /// </code>
        /// </summary>
        /// <param name="path">完整路径</param>
        /// <returns>目录路径</returns>
        public static string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 获取文件扩展名（包含点号）
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.GetExtension("movie.mp4") // 返回 ".mp4"
        /// PathHelper.GetExtension("archive.tar.gz") // 返回 ".gz"
        /// </code>
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件扩展名（包含点号）</returns>
        public static string GetExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return Path.GetExtension(path);
        }

        /// <summary>
        /// 安全地组合多个路径
        /// 自动处理不同平台的路径分隔符
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.CombinePath("C:\Videos", "movie.mp4") // 返回 "C:\Videos\movie.mp4"
        /// PathHelper.CombinePath("/home/user", "videos", "movie.mp4") // 返回 "/home/user/videos/movie.mp4"
        /// </code>
        /// </summary>
        /// <param name="paths">要组合的路径部分</param>
        /// <returns>组合后的完整路径</returns>
        public static string CombinePath(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            return Path.Combine(paths);
        }

        /// <summary>
        /// 获取文件夹大小（异步版本）
        /// 跨平台实现，不依赖特定操作系统命令
        ///
        /// 使用示例：
        /// <code>
        /// var size = await PathHelper.GetDirectorySizeAsync("C:\Videos");
        /// Console.WriteLine($"文件夹大小：{size / 1024 / 1024} MB");
        /// </code>
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>文件夹大小（字节）</returns>
        public static async Task<long> GetDirectorySizeAsync(string path)
        {
            return await Task.Run(() =>
            {
                if (!Directory.Exists(path))
                    return 0;

                long totalSize = 0;
                try
                {
                    string[] files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;
                    }

                    string[] subDirs = Directory.GetDirectories(path);
                    foreach (string subDir in subDirs)
                    {
                        totalSize += GetDirectorySizeAsync(subDir).Result;
                    }
                }
                catch
                {
                    // 忽略无法访问的文件夹或文件
                }

                return totalSize;
            });
        }

        /// <summary>
        /// 获取文件夹大小（同步版本）
        ///
        /// 注意：此方法可能较慢，建议在后台线程中调用
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>文件夹大小（字节）</returns>
        public static long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            long totalSize = 0;
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    var fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                }

                string[] subDirs = Directory.GetDirectories(path);
                foreach (string subDir in subDirs)
                {
                    totalSize += GetDirectorySize(subDir);
                }
            }
            catch
            {
                // 忽略无法访问的文件夹或文件
            }

            return totalSize;
        }

        /// <summary>
        /// 获取人类可读的文件大小字符串
        ///
        /// 使用示例：
        /// <code>
        /// PathHelper.FormatFileSize(1024) // 返回 "1 KB"
        /// PathHelper.FormatFileSize(1536) // 返回 "1.5 KB"
        /// PathHelper.FormatFileSize(1572864) // 返回 "1.5 MB"
        /// </code>
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化的大小字符串</returns>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 检查路径是否是有效的文件路径
        /// </summary>
        /// <param name="path">要检查的路径</param>
        /// <returns>true=有效文件路径，false=无效</returns>
        public static bool IsValidFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                return File.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查路径是否是有效的文件夹路径
        /// </summary>
        /// <param name="path">要检查的路径</param>
        /// <returns>true=有效文件夹路径，false=无效</returns>
        public static bool IsValidFolderPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                return Directory.Exists(path);
            }
            catch
            {
                return false;
            }
        }
    }
}
