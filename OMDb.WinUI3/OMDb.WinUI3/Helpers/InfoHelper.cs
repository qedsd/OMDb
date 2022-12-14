using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class InfoHelper
    {
        public static Grid WaitingGrid { get; set; }
        public static ProgressRing WaitingProgressRing { get; set; }
        public static InfoBar InfoBar { get; set; }
        public static Frame DialogFrame { get; set; }

        private static System.Timers.Timer Timer;
        private static void StartTimer()
        {
            if (Timer == null)
            {
                Timer = new System.Timers.Timer(3000);
                Timer.AutoReset = false;
                Timer.Elapsed += Timer_Elapsed;
            }
            Timer.Start();
        }
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                InfoBar.Message = string.Empty;
                InfoBar.IsOpen = false;
            });
        }
        public static void ShowMsg(string msg, bool autoClose = true)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                InfoBar.Severity = InfoBarSeverity.Informational;
                InfoBar.Message = msg;
                InfoBar.IsOpen = true;
                if (autoClose)
                {
                    StartTimer();
                }
            });
        }
        public static void ShowError(string msg, bool autoClose = true)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                InfoBar.Severity = InfoBarSeverity.Error;
                InfoBar.Message = msg;
                InfoBar.IsOpen = true;
                if (autoClose)
                {
                    StartTimer();
                }
            });
        }
        public static void ShowSuccess(string msg, bool autoClose = true)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                InfoBar.Severity = InfoBarSeverity.Success;
                InfoBar.Message = msg;
                InfoBar.IsOpen = true;
                if (autoClose)
                {
                    StartTimer();
                }
            });
        }

        public static void ShowWaiting()
        {
            ShowWaitingGrid();
            WaitingProgressRing.IsActive = true;
        }
        public static void HideWaiting()
        {
            HideWaitingGrid();
            WaitingProgressRing.IsActive = false;
        }

        public static void ShowWaitingGrid()
        {
            WaitingGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        public static void HideWaitingGrid()
        {
            WaitingGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}
