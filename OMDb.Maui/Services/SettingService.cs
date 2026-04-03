using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OMDb.Maui.Services
{
    /// <summary>
    /// 设置服务 - 管理应用程序的设置配置
    ///
    /// 主要功能：
    /// 1. Load - 从 settings.json 加载设置
    /// 2. Save - 保存设置到 settings.json
    /// 3. GetValue - 获取指定键的设置值
    /// 4. SetValueAsync - 异步设置指定键的值
    ///
    /// 配置文件位置：
    /// - Windows: %APPDATA%\OMDb\Configs\settings.json
    /// - macOS: ~/Library/Application Support/OMDb/Configs/settings.json
    /// - Linux: ~/.local/share/OMDb/Configs/settings.json
    ///
    /// 使用示例：
    /// <code>
    /// // 加载设置
    /// SettingService.Load();
    ///
    /// // 获取值
    /// string theme = SettingService.GetValue("Theme");
    ///
    /// // 设置值
    /// await SettingService.SetValueAsync("Theme", "Dark");
    /// </code>
    /// </summary>
    public static class SettingService
    {
        /// <summary>
        /// 设置文件路径
        /// 位于应用数据目录的 Configs 子目录下
        /// </summary>
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OMDb",
            "Configs",
            "settings.json"
        );

        /// <summary>
        /// 设置值字典
        /// Key=设置键名，Value=设置值（字符串）
        /// </summary>
        public static Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 保存设置到文件
        /// 将 Values 字典序列化为 JSON 并写入设置文件
        ///
        /// 注意：此方法是同步的，会阻塞 UI 线程
        /// 对于异步操作，请使用 SetValueAsync
        /// </summary>
        public static void Save()
        {
            try
            {
                // 序列化设置为 JSON
                string json = JsonConvert.SerializeObject(Values, Formatting.Indented);

                // 确保目录存在
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                // 记录错误但不抛出异常，避免影响应用运行
                System.Diagnostics.Debug.WriteLine($"保存设置失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 从文件加载设置
        /// 读取 settings.json 并反序列化为 Values 字典
        ///
        /// 如果文件不存在或为空，初始化空的 Values 字典
        /// 如果文件损坏，捕获异常并初始化空字典
        ///
        /// 注意：此方法是同步的，会阻塞 UI 线程
        /// </summary>
        public static void Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    Values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                    if (Values == null)
                    {
                        Values = new Dictionary<string, string>();
                    }
                }
                else
                {
                    // 文件不存在，初始化空字典
                    Values = new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                // 文件损坏或读取失败，初始化空字典
                System.Diagnostics.Debug.WriteLine($"加载设置失败：{ex.Message}");
                Values = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 获取指定键的设置值
        ///
        /// 使用示例：
        /// <code>
        /// string theme = SettingService.GetValue("Theme");
        /// if (theme != null)
        /// {
        ///     // 应用主题设置
        /// }
        /// </code>
        /// </summary>
        /// <param name="key">设置键名</param>
        /// <returns>设置值，如果键不存在则返回 null</returns>
        public static string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (Values.TryGetValue(key, out string value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 异步设置指定键的值
        /// 在后台线程中执行，不会阻塞 UI 线程
        ///
        /// 使用示例：
        /// <code>
        /// await SettingService.SetValueAsync("Theme", "Dark");
        /// await SettingService.SetValueAsync("Language", "zh-CN");
        /// </code>
        /// </summary>
        /// <param name="key">设置键名</param>
        /// <param name="value">设置值</param>
        /// <returns>Task</returns>
        public static async Task SetValueAsync(string key, string value)
        {
            await Task.Run(() =>
            {
                // 移除已存在的键（如果有）
                if (Values.ContainsKey(key))
                {
                    Values.Remove(key);
                }

                // 添加新值
                Values.Add(key, value);

                // 保存到文件
                Save();
            });
        }

        /// <summary>
        /// 删除指定键的设置
        ///
        /// 使用示例：
        /// <code>
        /// SettingService.Remove("TempSetting");
        /// </code>
        /// </summary>
        /// <param name="key">要删除的设置键名</param>
        public static void Remove(string key)
        {
            if (Values.ContainsKey(key))
            {
                Values.Remove(key);
                Save();
            }
        }

        /// <summary>
        /// 检查是否包含指定键的设置
        ///
        /// 使用示例：
        /// <code>
        /// if (SettingService.ContainsKey("Theme"))
        /// {
        ///     // 主题设置已存在
        /// }
        /// </code>
        /// </summary>
        /// <param name="key">设置键名</param>
        /// <returns>true=包含该键，false=不包含</returns>
        public static bool ContainsKey(string key)
        {
            return Values.ContainsKey(key);
        }

        /// <summary>
        /// 清空所有设置
        ///
        /// 注意：此操作不可逆，会删除所有已保存的设置
        /// </summary>
        public static void Clear()
        {
            Values.Clear();
            Save();
        }

        /// <summary>
        /// 获取所有设置键的列表
        ///
        /// 使用示例：
        /// <code>
        /// foreach (var key in SettingService.GetAllKeys())
        /// {
        ///     System.Diagnostics.Debug.WriteLine($"{key}: {SettingService.GetValue(key)}");
        /// }
        /// </code>
        /// </summary>
        /// <returns>所有设置键的集合</returns>
        public static ICollection<string> GetAllKeys()
        {
            return Values.Keys;
        }

        /// <summary>
        /// 获取所有设置值的列表
        ///
        /// 使用示例：
        /// <code>
        /// foreach (var value in SettingService.GetAllValues())
        /// {
        ///     System.Diagnostics.Debug.WriteLine(value);
        /// }
        /// </code>
        /// </summary>
        /// <returns>所有设置值的集合</returns>
        public static ICollection<string> GetAllValues()
        {
            return Values.Values;
        }
    }
}
