using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.Maui.Popups
{
    /// <summary>
    /// 选择对话框弹窗 - 显示选项列表供用户选择
    ///
    /// 主要功能：
    /// 1. 显示选项列表
    /// 2. 支持单选和多选
    /// 3. 返回用户选择的结果
    ///
    /// 使用示例：
    /// <code>
    /// // 单选
    /// var options = new List&lt;string&gt; { "选项 1", "选项 2", "选项 3" };
    /// string selected = await PickPopup.ShowSingleAsync("请选择", options);
    ///
    /// // 多选
    /// var selectedItems = await PickPopup.ShowMultipleAsync("请选择多项", options);
    ///
    /// // 带取消按钮
    /// string result = await PickPopup.ShowWithCancelAsync("选择类型", new[] { "A", "B", "C" }, "取消");
    /// </code>
    /// </summary>
    public static class PickPopup
    {
        /// <summary>
        /// 显示单项选择对话框
        /// 异步方法，等待用户选择
        ///
        /// 使用示例：
        /// <code>
        /// var options = new[] { "是", "否", "稍后" };
        /// string selected = await PickPopup.ShowSingleAsync("如何处理？", options);
        /// </code>
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="options">选项列表</param>
        /// <param name="cancelButton">取消按钮文本（null=无取消按钮）</param>
        /// <returns>用户选择的选项，取消时返回 null</returns>
        public static async Task<string> ShowSingleAsync(string title, IEnumerable<string> options, string cancelButton = null)
        {
            var optionsList = options.ToList();
            if (!optionsList.Any())
                return null;

            // 使用 MAUI 的 DisplayActionSheet
            if (cancelButton != null)
            {
                return await Application.Current.MainPage.DisplayActionSheet(title, cancelButton, null, optionsList.ToArray());
            }
            else
            {
                // 添加一个默认的"取消"选项
                optionsList.Add("取消");
                var result = await Application.Current.MainPage.DisplayActionSheet(title, null, null, optionsList.ToArray());
                return result == "取消" ? null : result;
            }
        }

        /// <summary>
        /// 显示单项选择对话框（带默认值）
        ///
        /// 使用示例：
        /// <code>
        /// var options = new[] { "红色", "绿色", "蓝色" };
        /// string selected = await PickPopup.ShowSingleAsync("选择颜色", options, "绿色");
        /// </code>
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="options">选项列表</param>
        /// <param name="defaultValue">默认选中的值</param>
        /// <param name="cancelButton">取消按钮文本</param>
        /// <returns>用户选择的选项，取消时返回 null</returns>
        public static async Task<string> ShowSingleWithDefaultAsync(string title, IEnumerable<string> options, string defaultValue, string cancelButton = "取消")
        {
            var optionsList = options.ToList();
            if (!optionsList.Any())
                return null;

            // 将默认值移到第一个位置
            if (defaultValue != null && optionsList.Contains(defaultValue))
            {
                optionsList.Remove(defaultValue);
                optionsList.Insert(0, defaultValue);
            }

            var result = await Application.Current.MainPage.DisplayActionSheet(title, cancelButton, null, optionsList.ToArray());
            return result == cancelButton ? null : result;
        }

        /// <summary>
        /// 显示多项选择对话框
        /// 返回用户选择的所有项
        ///
        /// 注意：MAUI 的 DisplayActionSheet 不支持多选
        /// 此方法使用多个单选对话框模拟多选
        ///
        /// 使用示例：
        /// <code>
        /// var options = new[] { "选项 1", "选项 2", "选项 3" };
        /// var selected = await PickPopup.ShowMultipleAsync("选择多项", options);
        /// </code>
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="options">选项列表</param>
        /// <returns>用户选择的所有选项</returns>
        public static async Task<List<string>> ShowMultipleAsync(string title, IEnumerable<string> options)
        {
            var optionsList = options.ToList();
            var selectedItems = new List<string>();

            foreach (var option in optionsList)
            {
                bool include = await Application.Current.MainPage.DisplayAlert(
                    title,
                    $"是否选择 \"{option}\"？",
                    "是",
                    "否"
                );

                if (include)
                {
                    selectedItems.Add(option);
                }
            }

            return selectedItems;
        }

        /// <summary>
        /// 显示选择对话框（带取消选项）
        /// 与 ShowSingleAsync 类似，但显式支持取消
        ///
        /// 使用示例：
        /// <code>
        /// var types = new[] { "电影", "电视剧", "纪录片" };
        /// string type = await PickPopup.ShowWithCancelAsync("选择类型", types, "取消");
        /// if (type != null)
        /// {
        ///     // 用户做出了选择
        /// }
        /// </code>
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="options">选项列表</param>
        /// <param name="cancelText">取消按钮文本</param>
        /// <returns>用户选择的选项，取消时返回 null</returns>
        public static async Task<string> ShowWithCancelAsync(string title, IEnumerable<string> options, string cancelText = "取消")
        {
            return await ShowSingleAsync(title, options, cancelText);
        }
    }
}
