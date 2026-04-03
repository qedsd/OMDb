using Newtonsoft.Json;
using OMDb.Core;
using OMDb.Core.DbModels;
using OMDb.Maui.Helpers;
using OMDb.Maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.Maui.Services
{
    /// <summary>
    /// 配置服务 - 管理应用程序的全局配置
    ///
    /// 主要功能：
    /// 1. 管理词条存储路径配置
    /// 2. 管理数据库连接配置
    /// 3. 管理资源文件夹路径
    /// 4. 加载和保存配置到 MCDb.db
    ///
    /// 配置文件位置：
    /// - Configs/MCDb.db - 存储配置信息
    ///
    /// 资源文件夹结构：
    /// - Resource/ - 资源根目录
    ///   - Image/ - 图片资源
    ///   - Audio/ - 音频资源
    ///   - Video/ - 视频资源
    ///     - Sub/ - 字幕资源
    ///   - More/ - 其他资源
    ///
    /// 使用示例：
    /// <code>
    /// // 加载配置
    /// ConfigService.Load();
    ///
    /// // 获取默认词条路径
    /// string defaultPath = ConfigService.DefaultEntryFolder;
    ///
    /// // 获取所有存储
    /// var storages = ConfigService.EnrtyStorages;
    /// </code>
    /// </summary>
    public static class ConfigService
    {
        /// <summary>
        /// 默认词条文件夹路径
        /// 相对于仓库路径
        /// </summary>
        public static string DefaultEntryFolder { get; set; }

        /// <summary>
        /// 资源文件夹路径
        /// </summary>
        public static string ResourceFolder { get; } = "Resource";

        /// <summary>
        /// 图片资源文件夹路径（相对于资源文件夹）
        /// </summary>
        public static string ImgFolder { get; } = Path.Combine("Resource", "Image");

        /// <summary>
        /// 音频资源文件夹路径（相对于资源文件夹）
        /// </summary>
        public static string AudioFolder { get; } = Path.Combine("Resource", "Audio");

        /// <summary>
        /// 视频资源文件夹路径（相对于资源文件夹）
        /// </summary>
        public static string VideoFolder { get; } = Path.Combine("Resource", "Video");

        /// <summary>
        /// 字幕资源文件夹路径（相对于视频资源文件夹）
        /// </summary>
        public static string SubFolder { get; } = Path.Combine("Resource", "Video", "Sub");

        /// <summary>
        /// 更多资源文件夹路径（相对于资源文件夹）
        /// </summary>
        public static string MoreFolder { get; } = Path.Combine("Resource", "More");

        /// <summary>
        /// 信息文件夹名称
        /// 用于存储词条元数据
        /// </summary>
        public static string InfoFolder { get; } = "Info";

        /// <summary>
        /// OMDb 配置文件夹名称
        /// 每个存储根目录下的隐藏文件夹
        /// </summary>
        public static string OMDbFolder { get; } = ".omdb";

        /// <summary>
        /// 存储数据库文件名
        /// </summary>
        public static string StorageDbName { get; } = "omdb.db";

        /// <summary>
        /// 默认封面图片路径
        /// 当词条没有封面时显示的默认图片
        /// </summary>
        public static string DefaultCover { get; private set; } = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Img",
            "DefaultCover.jpg"
        );

        /// <summary>
        /// 元数据文件名
        /// 每个词条目录下的 MetaData.json
        /// </summary>
        public static string MetadataFileName { get; } = "MetaData.json";

        /// <summary>
        /// 词条存储列表
        /// 包含所有已配置的词条仓库信息
        /// </summary>
        public static ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; }

        /// <summary>
        /// 加载配置
        /// 初始化数据库、设置和存储配置
        ///
        /// 调用顺序：
        /// 1. 初始化 MCDb.db 配置数据库
        /// 2. 加载 SettingService
        /// 3. 设置 FFmpeg 路径
        /// 4. 加载所有存储配置
        ///
        /// 使用示例：
        /// <code>
        /// // 应用启动时调用
        /// ConfigService.Load();
        /// </code>
        /// </summary>
        public static void Load()
        {
            try
            {
                // 初始化配置数据库
                string mcdbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "MCDb.db");
                Core.Config.InitMCDb(mcdbPath);

                // 加载设置服务
                SettingService.Load();

                // 设置 FFmpeg 可执行文件路径
                string ffmpegPath = Path.Combine(AppContext.BaseDirectory, "Assets", "FFmpeg");
                Core.Config.SetFFmpegExecutablesPath(ffmpegPath);

                // 加载存储配置
                _ = LoadStoragesAsync();
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"加载配置失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 加载所有存储配置
        /// 从配置数据库中读取所有存储并注册到 Core.Config
        ///
        /// 注意：这是异步方法，需要等待数据库操作完成
        /// </summary>
        public static async Task LoadStoragesAsync()
        {
            try
            {
                EnrtyStorages = new ObservableCollection<EnrtyStorage>();
                Core.Config.ClearDb();

                // 从配置数据库获取所有存储
                var lstStorage = await Core.Services.StorageService.GetAllStorageAsync(
                    Services.Settings.DbSelectorService.DbCurrentId
                );

                // 初始化存储列表
                EnrtyStorages.Clear();

                if (lstStorage != null)
                {
                    foreach (var item in lstStorage)
                    {
                        // 创建存储对象
                        var entryStorage = new EnrtyStorage
                        {
                            StorageId = item.Id,
                            StorageName = item.StorageName,
                            StoragePath = item.StoragePath,
                            CoverImg = item.CoverImg,
                            EntryCount = (int)item.EntryCount
                        };
                        EnrtyStorages.Add(entryStorage);

                        // 注册数据库
                        string dbPath = Path.Combine(
                            item.StoragePath,
                            OMDbFolder,
                            Services.Settings.DbSelectorService.DbCurrentName,
                            StorageDbName
                        );
                        Core.Config.AddDbFile(dbPath, item.StorageName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"加载存储配置失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 添加新的存储配置
        ///
        /// 使用示例：
        /// <code>
        /// await ConfigService.AddStorageAsync(
        ///     storageName: "我的片库",
        ///     storagePath: "D:\Movies"
        /// );
        /// </code>
        /// </summary>
        /// <param name="storageName">存储名称</param>
        /// <param name="storagePath">存储路径</param>
        /// <param name="coverImg">封面图片路径（可选）</param>
        /// <returns>true=成功，false=失败</returns>
        public static async Task<bool> AddStorageAsync(string storageName, string storagePath, string coverImg = null)
        {
            try
            {
                // 创建存储目录
                string omdbDir = Path.Combine(storagePath, OMDbFolder);
                if (!Directory.Exists(omdbDir))
                {
                    Directory.CreateDirectory(omdbDir);
                }

                // 创建数据库
                string dbPath = Path.Combine(omdbDir, Services.Settings.DbSelectorService.DbCurrentName, StorageDbName);
                string dbDir = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                // 添加到配置数据库（同步方法）
                var storageDb = new Core.DbModels.StorageDb
                {
                    StorageName = storageName,
                    StoragePath = storagePath,
                    CoverImg = coverImg
                };
                Core.Services.StorageService.AddStorage(storageDb);

                // 获取刚添加的存储
                var storages = await Core.Services.StorageService.GetAllStorageAsync(
                    Services.Settings.DbSelectorService.DbCurrentId
                );
                var storage = storages?.OrderByDescending(s => s.Id).FirstOrDefault();

                if (storage != null)
                {
                    // 注册数据库
                    Core.Config.AddDbFile(dbPath, storageName, false);

                    // 添加到列表
                    EnrtyStorages.Add(new EnrtyStorage
                    {
                        StorageId = storage.Id,
                        StorageName = storageName,
                        StoragePath = storagePath,
                        CoverImg = coverImg,
                        EntryCount = 0
                    });

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"添加存储失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 删除存储配置
        ///
        /// 注意：此方法只删除配置，不删除实际的文件
        /// 如果需要删除文件，请先调用 RemoveStorageFilesAsync
        ///
        /// 使用示例：
        /// <code>
        /// await ConfigService.RemoveStorageAsync(storageId);
        /// </code>
        /// </summary>
        /// <param name="storageId">存储 ID</param>
        /// <returns>true=成功，false=失败</returns>
        public static async Task<bool> RemoveStorageAsync(string storageId)
        {
            try
            {
                // 从列表获取存储信息
                var storage = EnrtyStorages.FirstOrDefault(s => s.StorageId == storageId);
                if (storage == null)
                    return false;

                // 使用 Core 的 RemoveStorage 方法（同步）
                Core.Services.StorageService.RemoveStorage(
                    Services.Settings.DbSelectorService.DbCurrentId,
                    storage.StorageName
                );

                // 从列表移除
                EnrtyStorages.Remove(storage);

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"删除存储失败：{ex.Message}");
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// 删除存储的所有文件
        /// 谨慎使用：此方法会永久删除所有文件
        ///
        /// 使用示例：
        /// <code>
        /// // 先确认用户是否确定删除
        /// bool confirmed = await InfoHelper.ShowQueryAsync("确认删除", "确定要删除此仓库的所有文件吗？");
        /// if (confirmed)
        /// {
        ///     await ConfigService.RemoveStorageFilesAsync(storageId);
        /// }
        /// </code>
        /// </summary>
        /// <param name="storageId">存储 ID</param>
        /// <returns>true=成功，false=失败</returns>
        public static async Task<bool> RemoveStorageFilesAsync(string storageId)
        {
            try
            {
                var storage = EnrtyStorages.FirstOrDefault(s => s.StorageId == storageId);
                if (storage != null && Directory.Exists(storage.StoragePath))
                {
                    await Task.Run(() =>
                    {
                        Directory.Delete(storage.StoragePath, true);
                    });
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                InfoHelper.ShowErrorAsync($"删除文件失败：{ex.Message}");
                return false;
            }
        }
    }
}
