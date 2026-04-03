using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace OMDb.Maui.Popups
{
    /// <summary>
    /// 消息对话框弹窗 - 显示消息提示
    ///
    /// 主要功能：
    /// 1. 显示标题和内容
    /// 2. 提供单个"确定"按钮
    /// 3. 用于显示信息、警告、错误等消息
    ///
    /// 使用示例：
    /// <code>
    /// // 显示普通消息
    /// await MessagePopup.ShowAsync("操作已完成");
    ///
    /// // 显示带标题的消息
    /// await MessagePopup.ShowAsync("提示", "数据已保存");
    ///
    /// // 显示错误消息
    /// await MessagePopup.ShowErrorAsync("保存失败：磁盘空间不足");
    ///
    /// // 显示成功消息
    /// await MessagePopup.ShowSuccessAsync("上传成功！");
    /// </code>
    ///
    /// 注意：MAUI 版本直接使用 DisplayAlert，不需要自定义 Popup
    /// 如需自定义外观，可以使用 CommunityToolkit.Maui.Popup
    /// </summary>
    public static class MessagePopup
    {
        /// <summary>
        /// 显示消息对话框
        /// 异步方法，等待用户点击确定
        ///
        /// 使用示例：
        /// <code>
        /// await MessagePopup.ShowAsync("这是一条消息");
        /// await MessagePopup.ShowAsync("提示", "数据已保存");
        /// </code>
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题（可选）</param>
        /// <param name="confirmButton">确定按钮文本（默认"确定"）</param>
        /// <returns>Task</returns>
        public static async Task ShowAsync(string message, string title = "提示", string confirmButton = "确定")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, confirmButton);
        }

        /// <summary>
        /// 显示错误消息对话框
        /// 使用红色主题或错误图标（如果平台支持）
        ///
        /// 使用示例：
        /// <code>
        /// await MessagePopup.ShowErrorAsync("连接服务器失败");
        /// await MessagePopup.ShowErrorAsync("保存失败：磁盘空间不足");
        /// </code>
        /// </summary>
        /// <param name="message">错误消息内容</param>
        /// <param name="title">标题（默认"错误"）</param>
        /// <returns>Task</returns>
        public static async Task ShowErrorAsync(string message, string title = "错误")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "确定");
        }

        /// <summary>
        /// 显示成功消息对话框
        /// 使用绿色主题或成功图标（如果平台支持）
        ///
        /// 使用示例：
        /// <code>
        /// await MessagePopup.ShowSuccessAsync("数据保存成功！");
        /// await MessagePopup.ShowSuccessAsync("上传完成", "操作成功");
        /// </code>
        /// </summary>
        /// <param name="message">成功消息内容</param>
        /// <param name="title">标题（默认"成功"）</param>
        /// <returns>Task</returns>
        public static async Task ShowSuccessAsync(string message, string title = "成功")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "确定");
        }

        /// <summary>
        /// 显示警告消息对话框
        /// 使用黄色主题或警告图标（如果平台支持）
        ///
        /// 使用示例：
        /// <code>
        /// await MessagePopup.ShowWarningAsync("文件即将被覆盖");
        /// await MessagePopup.ShowWarningAsync("此操作不可撤销", "警告");
        /// </code>
        /// </summary>
        /// <param name="message">警告消息内容</param>
        /// <param name="title">标题（默认"警告"）</param>
        /// <returns>Task</returns>
        public static async Task ShowWarningAsync(string message, string title = "警告")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "确定");
        }
    }
}
