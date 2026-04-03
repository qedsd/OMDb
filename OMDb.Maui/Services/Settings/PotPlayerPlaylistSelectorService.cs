using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.Maui.Services.Settings
{
    /// <summary>
    /// PotPlayer 播放列表选择器服务 - 管理 PotPlayer 播放列表路径配置
    ///
    /// 主要功能：
    /// 1. 保存和加载 PotPlayer 播放列表文件路径
    /// 2. 异步保存路径设置
    ///
    /// 用途：
    /// - 用于将播放列表导出到 PotPlayer
    /// - 保存用户配置的播放列表文件位置
    ///
    /// 配置存储：
    /// - 键名："PotPlayerPlaylistPath"
    /// - 值：播放列表文件的完整路径
    ///
    /// 使用示例：
    /// <code>
    /// // 初始化（应用启动时调用）
    /// PotPlayerPlaylistSelectorService.Initialize();
    ///
    /// // 获取当前播放列表路径
    /// string playlistPath = PotPlayerPlaylistSelectorService.PlaylistPath;
    ///
    /// // 设置新的播放列表路径
    /// await PotPlayerPlaylistSelectorService.SetAsync("C:\Playlists\myplaylist.pls");
    /// </code>
    /// </summary>
    public static class PotPlayerPlaylistSelectorService
    {
        /// <summary>
        /// 设置键名
        /// 用于在 SettingService 中存储播放列表路径
        /// </summary>
        private const string Key = "PotPlayerPlaylistPath";

        /// <summary>
        /// 播放列表文件路径
        /// 保存用户配置的 PotPlayer 播放列表文件位置
        ///
        /// 默认值：空字符串
        /// </summary>
        public static string PlaylistPath { get; private set; } = string.Empty;

        /// <summary>
        /// 初始化播放列表选择器
        /// 从配置文件加载上次保存的播放列表路径
        ///
        /// 如果配置文件不存在或值为空，PlaylistPath 保持为空字符串
        ///
        /// 注意：此方法应在应用启动时调用
        /// </summary>
        public static void Initialize()
        {
            PlaylistPath = LoadFromSettings();
        }

        /// <summary>
        /// 设置播放列表路径
        /// 异步方法，保存到配置文件
        ///
        /// 使用示例：
        /// <code>
        /// // 用户选择文件后保存
        /// string selectedPath = await PickFileAsync();
        /// if (selectedPath != null)
        /// {
        ///     await PotPlayerPlaylistSelectorService.SetAsync(selectedPath);
        /// }
        /// </code>
        /// </summary>
        /// <param name="value">播放列表文件路径</param>
        /// <returns>Task</returns>
        public static async Task SetAsync(string value)
        {
            PlaylistPath = value;
            await SaveInSettingsAsync(value);
        }

        /// <summary>
        /// 从配置文件加载播放列表路径
        /// 私有方法，仅被 Initialize 调用
        ///
        /// 读取逻辑：
        /// 1. 从 SettingService 获取值
        /// 2. 如果值存在，返回该值
        /// 3. 如果值不存在，返回 null
        ///
        /// </summary>
        /// <returns>播放列表路径，如果未配置则返回 null</returns>
        private static string LoadFromSettings()
        {
            return SettingService.GetValue(Key);
        }

        /// <summary>
        /// 保存到配置文件
        /// 私有方法，仅被 SetAsync 调用
        ///
        /// 将播放列表路径保存到 SettingService
        /// </summary>
        /// <param name="value">播放列表路径</param>
        /// <returns>Task</returns>
        private static async Task SaveInSettingsAsync(string value)
        {
            await SettingService.SetValueAsync(Key, value);
        }

        /// <summary>
        /// 清除播放列表路径配置
        /// 将路径设置为空字符串并保存
        ///
        /// 使用示例：
        /// <code>
        /// // 用户点击"清除配置"按钮时
        /// PotPlayerPlaylistSelectorService.Clear();
        /// </code>
        /// </summary>
        public static async Task ClearAsync()
        {
            PlaylistPath = string.Empty;
            await SaveInSettingsAsync(string.Empty);
        }

        /// <summary>
        /// 检查是否已配置播放列表路径
        ///
        /// 使用示例：
        /// <code>
        /// if (PotPlayerPlaylistSelectorService.IsConfigured())
        /// {
        ///     // 可以使用已配置的路径
        /// }
        /// else
        /// {
        ///     // 需要用户先配置路径
        /// }
        /// </code>
        /// </summary>
        /// <returns>true=已配置，false=未配置</returns>
        public static bool IsConfigured()
        {
            return !string.IsNullOrEmpty(PlaylistPath);
        }
    }
}
