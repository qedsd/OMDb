using Microsoft.UI.Xaml.Controls;
using OMDb.WinUI3.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class DialogHelper
    {
        public static MyContentDialog Current { get; set; }
        public static InfoBar InfoBar { get; set; }
        public static void ShowMsg(string msg)
        {
            InfoBar.Severity = InfoBarSeverity.Informational;
            InfoBar.Message = msg;
            InfoBar.IsOpen = true;
            //System.Timers.Timer timer = new System.Timers.Timer(3000);
            //timer.AutoReset = false;
            //timer.Elapsed += Timer_Elapsed;
        }

        public static void ShowSuccess(string msg)
        {
            InfoBar.Severity = InfoBarSeverity.Success;
            InfoBar.Message = msg;
            InfoBar.IsOpen = true;
            //System.Timers.Timer timer = new System.Timers.Timer(3000);
            //timer.AutoReset = false;
            //timer.Elapsed += Timer_Elapsed;
        }

        /*private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {

            });
        }*/

        public static void ShowError(string msg)
        {
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.Message = msg;
            InfoBar.IsOpen = true;
        }
    }
}
