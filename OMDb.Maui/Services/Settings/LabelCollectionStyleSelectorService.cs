using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.Maui.Services.Settings
{
    /// <summary>
    /// 标签合集样式选择器服务 - 管理标签合集的显示样式
    ///
    /// 主要功能：
    /// 1. 保存和加载标签合集的显示样式（列表/网格）
    /// 2. 提供 IsList 属性判断当前是否为列表模式
    /// 3. 异步保存样式设置
    ///
    /// 样式值：
    /// - 0 = List（列表视图）
    /// - 1 = GridView（网格视图）
    ///
    /// 配置存储：
    /// - 键名："LabelCollectionStyle"
    /// - 值：整数字符串（"0" 或 "1"）
    ///
    /// 使用示例：
    /// <code>
    /// // 初始化（应用启动时调用）
    /// LabelCollectionStyleSelectorService.Initialize();
    ///
    /// // 检查当前样式
    /// if (LabelCollectionStyleSelectorService.IsList)
    /// {
    ///     // 当前是列表模式
    /// }
    ///
    /// // 切换为网格视图
    /// await LabelCollectionStyleSelectorService.SetAsync(1);
    ///
    /// // 获取当前样式值
    /// int currentStyle = LabelCollectionStyleSelectorService.Style;
    /// </code>
    /// </summary>
    public static class LabelCollectionStyleSelectorService
    {
        /// <summary>
        /// 设置键名
        /// 用于在 SettingService 中存储样式值
        /// </summary>
        private const string Key = "LabelCollectionStyle";

        /// <summary>
        /// 当前样式值
        /// 0 = List（列表视图）
        /// 1 = GridView（网格视图）
        ///
        /// 默认值：0（列表视图）
        /// </summary>
        public static int Style { get; private set; }

        /// <summary>
        /// 是否为列表模式
        /// 只读属性，根据 Style 值返回结果
        ///
        /// 使用示例：
        /// <code>
        /// if (LabelCollectionStyleSelectorService.IsList)
        /// {
        ///     // 使用列表布局
        /// }
        /// else
        /// {
        ///     // 使用网格布局
        /// }
        /// </code>
        /// </summary>
        public static bool IsList => Style == 0;

        /// <summary>
        /// 初始化样式选择器
        /// 从配置文件加载上次保存的样式设置
        ///
        /// 如果配置文件不存在或值为空，使用默认值（0 = 列表视图）
        ///
        /// 注意：此方法应与应用启动时调用
        /// </summary>
        public static void Initialize()
        {
            Style = LoadFromSettings();
        }

        /// <summary>
        /// 设置样式
        /// 异步方法，保存到配置文件
        ///
        /// 使用示例：
        /// <code>
        /// // 切换为网格视图
        /// await LabelCollectionStyleSelectorService.SetAsync(1);
        ///
        /// // 切换为列表视图
        /// await LabelCollectionStyleSelectorService.SetAsync(0);
        /// </code>
        /// </summary>
        /// <param name="style">样式值（0=列表，1=网格）</param>
        /// <returns>Task</returns>
        public static async Task SetAsync(int style)
        {
            Style = style;
            await SaveInSettingsAsync(style);
        }

        /// <summary>
        /// 从配置文件加载样式值
        /// 私有方法，仅被 Initialize 调用
        ///
        /// 读取逻辑：
        /// 1. 从 SettingService 获取值
        /// 2. 如果值存在且有效，解析为整数
        /// 3. 如果值不存在或无效，返回默认值 0
        ///
        /// </summary>
        /// <returns>样式值（0=列表，1=网格）</returns>
        private static int LoadFromSettings()
        {
            string styleValue = SettingService.GetValue(Key);

            if (!string.IsNullOrEmpty(styleValue) && int.TryParse(styleValue, out int style))
            {
                return style;
            }

            // 默认值：列表视图
            return 0;
        }

        /// <summary>
        /// 保存到配置文件
        /// 私有方法，仅被 SetAsync 调用
        ///
        /// 将样式值转换为字符串并保存到 SettingService
        /// </summary>
        /// <param name="style">样式值</param>
        /// <returns>Task</returns>
        private static async Task SaveInSettingsAsync(int style)
        {
            await SettingService.SetValueAsync(Key, style.ToString());
        }

        /// <summary>
        /// 切换样式
        /// 在列表和网格之间切换
        ///
        /// 使用示例：
        /// <code>
        /// // 点击切换按钮时调用
        /// LabelCollectionStyleSelectorService.Toggle();
        /// </code>
        /// </summary>
        /// <returns>Task</returns>
        public static async Task ToggleAsync()
        {
            int newStyle = IsList ? 1 : 0;
            await SetAsync(newStyle);
        }
    }
}
