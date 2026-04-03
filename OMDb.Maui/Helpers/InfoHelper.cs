using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace OMDb.Maui.Helpers
{
    /// <summary>
    /// 信息提示助手 - 提供统一的消息提示、错误显示、确认对话框等功能
    ///
    /// 主要功能：
    /// 1. ShowMsgAsync - 显示普通消息提示
    /// 2. ShowErrorAsync - 显示错误消息
    /// 3. ShowSuccessAsync - 显示成功消息
    /// 4. ShowQueryAsync - 显示确认对话框
    /// 5. ShowWaiting/HideWaiting - 显示/隐藏等待动画（待实现）
    /// </summary>
    public static class InfoHelper
    {
        private static System.Timers.Timer _timer;

        /// <summary>
        /// 显示普通消息提示
        /// </summary>
        /// <param name="msg">要显示的消息内容</param>
        /// <param name="autoClose">是否自动关闭（未实现）</param>
        /// <returns>Task</returns>
        public static async Task ShowMsgAsync(string msg, bool autoClose = true)
        {
            await Application.Current.MainPage.DisplayAlert("提示", msg, "确定");
        }

        /// <summary>
        /// 显示错误消息提示
        /// </summary>
        /// <param name="msg">要显示的错误消息</param>
        /// <param name="autoClose">是否自动关闭（未实现）</param>
        /// <returns>Task</returns>
        public static async Task ShowErrorAsync(string msg, bool autoClose = false)
        {
            await Application.Current.MainPage.DisplayAlert("错误", msg, "确定");
        }

        /// <summary>
        /// 显示成功消息提示
        /// </summary>
        /// <param name="msg">要显示的成功消息</param>
        /// <param name="autoClose">是否自动关闭（未实现）</param>
        /// <returns>Task</returns>
        public static async Task ShowSuccessAsync(string msg, bool autoClose = true)
        {
            await Application.Current.MainPage.DisplayAlert("成功", msg, "确定");
        }

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="content">对话框内容</param>
        /// <returns>用户选择结果：true=确定，false=取消</returns>
        public static async Task<bool> ShowQueryAsync(string title, string content)
        {
            return await Application.Current.MainPage.DisplayAlert(title, content, "确定", "取消");
        }

        /// <summary>
        /// 显示等待动画
        /// TODO: 需要实现加载指示器
        /// </summary>
        public static void ShowWaiting()
        {
            // TODO: 显示等待动画
        }

        /// <summary>
        /// 隐藏等待动画
        /// TODO: 需要实现加载指示器
        /// </summary>
        public static void HideWaiting()
        {
            // TODO: 隐藏等待动画
        }
    }
}
