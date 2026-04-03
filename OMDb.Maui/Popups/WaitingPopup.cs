using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace OMDb.Maui.Popups
{
    /// <summary>
    /// 等待对话框弹窗 - 显示加载指示器
    ///
    /// 主要功能：
    /// 1. 显示加载动画和提示消息
    /// 2. 支持关闭等待对话框
    /// 3. 支持超时自动关闭
    ///
    /// 使用示例：
    /// <code>
    /// // 显示等待对话框
    /// WaitingPopup.Show("正在加载数据...");
    ///
    /// // 执行耗时操作
    /// await DoSomethingAsync();
    ///
    /// // 关闭等待对话框
    /// WaitingPopup.Hide();
    ///
    /// // 使用 using 语句自动关闭
    /// using (WaitingPopup.Show("处理中..."))
    /// {
    ///     await DoSomethingAsync();
    /// }
    /// </code>
    ///
    /// 注意：当前实现为占位符，实际使用需要结合 CommunityToolkit.Maui.Popup
    /// </summary>
    public static class WaitingPopup
    {
        /// <summary>
        /// 当前显示的等待对话框
        /// </summary>
        private static Page _currentPage;

        /// <summary>
        /// 显示等待对话框
        /// 非阻塞方法，立即返回
        ///
        /// 使用示例：
        /// <code>
        /// WaitingPopup.Show("正在处理...");
        /// await Task.Run(() => HeavyWork());
        /// WaitingPopup.Hide();
        /// </code>
        /// </summary>
        /// <param name="message">等待消息</param>
        public static void Show(string message = "请稍候...")
        {
            // TODO: 实现真正的等待对话框
            // 目前使用 InfoHelper.ShowWaiting 占位
            Helpers.InfoHelper.ShowWaiting();
        }

        /// <summary>
        /// 显示等待对话框（带进度）
        /// 非阻塞方法，立即返回
        ///
        /// 使用示例：
        /// <code>
        /// WaitingPopup.ShowWithProgress("正在下载...", 0.3);
        /// </code>
        /// </summary>
        /// <param name="message">等待消息</param>
        /// <param name="progress">进度值（0-1 之间）</param>
        public static void ShowWithProgress(string message, double progress)
        {
            // TODO: 实现带进度的等待对话框
            System.Diagnostics.Debug.WriteLine($"{message} {progress:P}");
        }

        /// <summary>
        /// 隐藏等待对话框
        ///
        /// 使用示例：
        /// <code>
        /// WaitingPopup.Show("处理中...");
        /// await DoWorkAsync();
        /// WaitingPopup.Hide();
        /// </code>
        /// </summary>
        public static void Hide()
        {
            // TODO: 实现真正的隐藏逻辑
            Helpers.InfoHelper.HideWaiting();
        }

        /// <summary>
        /// 显示等待对话框并返回 disposable 对象
        /// 用于 using 语句，自动关闭对话框
        ///
        /// 使用示例：
        /// <code>
        /// using (WaitingPopup.Begin("处理中..."))
        /// {
        ///     await DoWorkAsync();
        /// }
        /// // 自动关闭对话框
        /// </code>
        /// </summary>
        /// <param name="message">等待消息</param>
        /// <returns>IDisposable 对象</returns>
        public static IDisposable Begin(string message = "请稍候...")
        {
            Show(message);
            return new WaitingDisposable();
        }

        /// <summary>
        /// Disposable 包装器，用于 using 语句
        /// </summary>
        private class WaitingDisposable : IDisposable
        {
            public void Dispose()
            {
                Hide();
            }
        }

        /// <summary>
        /// 显示等待对话框并在指定时间后自动关闭
        ///
        /// 使用示例：
        /// <code>
        /// await WaitingPopup.ShowWithTimeout("处理中...", TimeSpan.FromSeconds(5));
        /// // 5 秒后自动关闭
        /// </code>
        /// </summary>
        /// <param name="message">等待消息</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>Task</returns>
        public static async Task ShowWithTimeout(string message, TimeSpan timeout)
        {
            Show(message);
            await Task.Delay(timeout);
            Hide();
        }
    }
}
