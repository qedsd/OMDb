using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Dialogs
{
    public partial class DialogBase:Page
    {
        private bool activating = false;
        public virtual void Show()
        {
            activating = true;
            Helpers.InfoHelper.ShowWaitingGrid();
            Helpers.InfoHelper.DialogFrame.Content = this;
            Helpers.InfoHelper.DialogFrame.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        public virtual async Task ShowAsync()
        {
            Show();
            await Task.Run(() =>
            {
                while(activating)
                {
                    Thread.Sleep(200);
                }
            });
        }
        public virtual void Close()
        {
            activating = false;
            Helpers.InfoHelper.HideWaitingGrid();
            Helpers.InfoHelper.DialogFrame.Content = null;
            Helpers.InfoHelper.DialogFrame.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}
