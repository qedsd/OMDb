using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OMDb.WinUI3.Wins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace OMDb.WinUI3.Views.Tools
{
    public partial class ToolPageBase : Page
    {
        public ToolPageBase(string toolName)
        {
            ToolName = toolName;
        }
        public string ToolName { get; set; }
        public ToolWindow Window { get; set; }
        public void Show()
        {
            ToolWindow window = new ToolWindow();
            window.Content = this;
            window.Head = ToolName;
            Window = window;
            window.Activate();
        }

        public void ShowMsg(string msg, bool autoClose = true)
        {
            Window.ShowMsg(msg, autoClose);
        }
        public void ShowError(string msg, bool autoClose = true)
        {
            Window.ShowError(msg, autoClose);
        }
        public void ShowSuccess(string msg, bool autoClose = true)
        {
            Window.ShowSuccess(msg, autoClose);
        }
        public void ShowWaiting()
        {
            Window.ShowWaiting();
        }
        public void HideWaiting()
        {
            Window.HideWaiting();
        }
    }
}
