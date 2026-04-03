using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace OMDb.Maui.Popups
{
    /// <summary>
    /// 查询对话框弹窗 - 显示确认对话框
    ///
    /// 主要功能：
    /// 1. 显示标题和内容
    /// 2. 提供"确认"和"取消"两个按钮
    /// 3. 返回用户选择结果
    ///
    /// 使用示例：
    /// <code>
    /// // 简单用法
    /// bool result = await QueryPopup.ShowAsync("确认删除", "确定要删除这个项目吗？");
    /// if (result)
    /// {
    ///     // 用户点击了确认
    /// }
    ///
    /// // 在 ViewModel 中使用
    /// var confirm = await Application.Current.MainPage.DisplayAlert("确认", "是否确认删除？", "是", "否");
    /// </code>
    ///
    /// 注意：MAUI 版本直接使用 DisplayAlert，不需要自定义 Popup
    /// 如需自定义外观，可以使用 CommunityToolkit.Maui.Popup
    /// </summary>
    public static class QueryPopup
    {
        /// <summary>
        /// 显示查询对话框
        /// 异步方法，等待用户选择
        ///
        /// 使用示例：
        /// <code>
        /// bool confirmed = await QueryPopup.ShowAsync(
        ///     title: "确认删除",
        ///     message: "确定要删除这个项目吗？"
        /// );
        /// </code>
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">对话框内容</param>
        /// <param name="confirmButton">确认按钮文本（默认"确定"）</param>
        /// <param name="cancelButton">取消按钮文本（默认"取消"）</param>
        /// <returns>用户选择结果：true=确认，false=取消</returns>
        public static async Task<bool> ShowAsync(string title, string message, string confirmButton = "确定", string cancelButton = "取消")
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, confirmButton, cancelButton);
        }
    }
}
