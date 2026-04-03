using Newtonsoft.Json;
using OMDb.Maui.Models;
using OMDb.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.Maui.Services.Settings
{
    /// <summary>
    /// 主页项目配置服务 - 管理主页显示的项目配置
    ///
    /// 主要功能：
    /// 1. 管理主页上显示的所有可用项目
    /// 2. 管理已激活（显示）的项目列表
    /// 3. 管理未激活（隐藏）的项目列表
    /// 4. 保存和加载用户的项目配置
    ///
    /// 配置存储：
    /// - 键名："HomeItemConfig"
    /// - 值：JSON 序列化的 HomeItemConfig 列表
    ///
    /// 默认项目：
    /// - 摘录台词
    /// - 最近观看视频
    /// - 最近观看词条
    /// - 最近更新词条
    /// - 随机词条
    /// - 统计信息
    ///
    /// 使用示例：
    /// <code>
    /// // 初始化（应用启动时调用）
    /// HomeItemConfigsService.Initialize();
    ///
    /// // 获取所有项目
    /// var allItems = HomeItemConfigsService.AllItems;
    ///
    /// // 获取已激活的项目
    /// var activeItems = HomeItemConfigsService.ActiveItems;
    ///
    /// // 设置激活的项目
    /// await HomeItemConfigsService.SetAsync(selectedItems);
    /// </code>
    /// </summary>
    public static class HomeItemConfigsService
    {
        /// <summary>
        /// 设置键名
        /// 用于在 SettingService 中存储配置
        /// </summary>
        private const string Key = "HomeItemConfig";

        /// <summary>
        /// 已激活的项目列表
        /// 这些项目会显示在主页上
        ///
        /// 用户可以自定义哪些项目显示，哪些隐藏
        /// </summary>
        public static List<HomeItemConfig> ActiveItems { get; private set; }

        /// <summary>
        /// 未激活的项目列表
        /// 这些项目存在但不显示在主页上
        ///
        /// 用户可以随时重新激活这些项目
        /// </summary>
        public static List<HomeItemConfig> InactiveItems { get; private set; }

        /// <summary>
        /// 所有可用项目列表
        /// 包含 ActiveItems 和 InactiveItems 的并集
        ///
        /// 用于配置界面显示所有可选项
        /// </summary>
        public static List<HomeItemConfig> AllItems { get; private set; }

        /// <summary>
        /// 初始化主页项目配置
        /// 从配置文件加载上次保存的配置
        ///
        /// 执行步骤：
        /// 1. 初始化所有可用项目（AllItems）
        /// 2. 从 SettingService 加载配置
        /// 3. 如果配置不存在，使用默认值（所有项目都激活）
        /// 4. 如果配置存在，反序列化并分离为 ActiveItems 和 InactiveItems
        ///
        /// 注意：此方法应在应用启动时调用
        /// </summary>
        public static void Initialize()
        {
            LoadFromSettings();
        }

        /// <summary>
        /// 设置已激活的项目
        /// 异步方法，保存到配置文件
        ///
        /// 执行逻辑：
        /// 1. 将传入的项目列表设置为 ActiveItems
        /// 2. 从 AllItems 中找出不在 ActiveItems 中的项目，设置为 InactiveItems
        /// 3. 保存到配置文件
        ///
        /// 使用示例：
        /// <code>
        /// // 用户配置后保存
        /// var selectedItems = new List&lt;HomeItemConfig&gt;
        /// {
        ///     new HomeItemConfig("最近观看视频", typeof(HomePage)),
        ///     new HomeItemConfig("统计信息", typeof(ToolsPage))
        /// };
        /// await HomeItemConfigsService.SetAsync(selectedItems);
        /// </code>
        /// </summary>
        /// <param name="activeItems">要激活的项目列表</param>
        /// <returns>Task</returns>
        public static async Task SetAsync(List<HomeItemConfig> activeItems)
        {
            ActiveItems = activeItems.ToList();

            // 清空并重新填充 InactiveItems
            InactiveItems.Clear();
            foreach (var item in AllItems)
            {
                // 如果项目不在 ActiveItems 中，添加到 InactiveItems
                if (ActiveItems.FirstOrDefault(p => p.Name == item.Name) == null)
                {
                    InactiveItems.Add(item);
                }
            }

            // 保存到配置文件
            await SaveInSettingsAsync(activeItems);
        }

        /// <summary>
        /// 从配置文件加载配置
        /// 私有方法，仅被 Initialize 调用
        ///
        /// 执行逻辑：
        /// 1. 初始化 AllItems 列表，包含所有可用项目
        /// 2. 从 SettingService 获取 JSON 配置
        /// 3. 如果配置为空，ActiveItems 设置为 AllItems（默认全部激活）
        /// 4. 如果配置存在，反序列化为 ActiveItems
        /// 5. 将不在 ActiveItems 中的项目添加到 InactiveItems
        /// </summary>
        private static void LoadFromSettings()
        {
            // 初始化所有可用项目
            AllItems = new List<HomeItemConfig>();

            // 注意：以下是示例项目，实际项目需要根据 MAUI 版本调整
            // 由于 MAUI 版本可能没有所有 WinUI3 的页面，需要根据实际情况调整
            AllItems.Add(new HomeItemConfig("摘录台词", typeof(EntryHomePage)));
            AllItems.Add(new HomeItemConfig("最近观看视频", typeof(HomePage)));
            AllItems.Add(new HomeItemConfig("最近观看词条", typeof(HomePage)));
            AllItems.Add(new HomeItemConfig("最近更新词条", typeof(HomePage)));
            AllItems.Add(new HomeItemConfig("随机词条", typeof(ToolsPage)));
            AllItems.Add(new HomeItemConfig("统计信息", typeof(ToolsPage)));

            // 初始化 InactiveItems
            InactiveItems = new List<HomeItemConfig>();

            // 从配置文件读取
            string json = SettingService.GetValue(Key);

            if (string.IsNullOrEmpty(json))
            {
                // 配置不存在，默认全部激活
                ActiveItems = AllItems.ToList();
            }
            else
            {
                // 反序列化配置
                ActiveItems = JsonConvert.DeserializeObject<List<HomeItemConfig>>(json);

                // 将不在 ActiveItems 中的项目添加到 InactiveItems
                foreach (var item in AllItems)
                {
                    if (ActiveItems.FirstOrDefault(p => p.Name == item.Name) == null)
                    {
                        InactiveItems.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 保存到配置文件
        /// 私有方法，仅被 SetAsync 调用
        ///
        /// 将 ActiveItems 列表序列化为 JSON 并保存到 SettingService
        /// </summary>
        /// <param name="activeItems">要保存的激活项目列表</param>
        /// <returns>Task</returns>
        private static async Task SaveInSettingsAsync(List<HomeItemConfig> activeItems)
        {
            string json = JsonConvert.SerializeObject(activeItems);
            await SettingService.SetValueAsync(Key, json);
        }

        /// <summary>
        /// 激活指定项目
        /// 将项目从 InactiveItems 移动到 ActiveItems
        ///
        /// 使用示例：
        /// <code>
        /// var item = HomeItemConfigsService.AllItems.First(p => p.Name == "统计信息");
        /// await HomeItemConfigsService.ActivateItemAsync(item);
        /// </code>
        /// </summary>
        /// <param name="item">要激活的项目</param>
        /// <returns>Task</returns>
        public static async Task ActivateItemAsync(HomeItemConfig item)
        {
            if (item == null) return;

            // 从 InactiveItems 移除
            InactiveItems.RemoveAll(p => p.Name == item.Name);

            // 添加到 ActiveItems（如果不存在）
            if (ActiveItems.FirstOrDefault(p => p.Name == item.Name) == null)
            {
                ActiveItems.Add(item);
                await SetAsync(ActiveItems);
            }
        }

        /// <summary>
        /// 停用指定项目
        /// 将项目从 ActiveItems 移动到 InactiveItems
        ///
        /// 使用示例：
        /// <code>
        /// var item = HomeItemConfigsService.ActiveItems.First(p => p.Name == "统计信息");
        /// await HomeItemConfigsService.DeactivateItemAsync(item);
        /// </code>
        /// </summary>
        /// <param name="item">要停用的项目</param>
        /// <returns>Task</returns>
        public static async Task DeactivateItemAsync(HomeItemConfig item)
        {
            if (item == null) return;

            // 从 ActiveItems 移除
            ActiveItems.RemoveAll(p => p.Name == item.Name);

            // 添加到 InactiveItems（如果不存在）
            if (InactiveItems.FirstOrDefault(p => p.Name == item.Name) == null)
            {
                InactiveItems.Add(item);
                await SetAsync(ActiveItems);
            }
        }

        /// <summary>
        /// 检查项目是否已激活
        ///
        /// 使用示例：
        /// <code>
        /// if (HomeItemConfigsService.IsActive("统计信息"))
        /// {
        ///     // 该项目当前显示在主页上
        /// }
        /// </code>
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <returns>true=已激活，false=未激活</returns>
        public static bool IsActive(string itemName)
        {
            return ActiveItems?.Any(p => p.Name == itemName) ?? false;
        }

        /// <summary>
        /// 重置为默认配置
        /// 将所有项目设置为激活状态
        ///
        /// 使用示例：
        /// <code>
        /// // 用户点击"恢复默认设置"按钮
        /// await HomeItemConfigsService.ResetToDefaultsAsync();
        /// </code>
        /// </summary>
        public static async Task ResetToDefaultsAsync()
        {
            await SetAsync(AllItems.ToList());
        }
    }
}
