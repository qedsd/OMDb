using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OMDb.WinUI3.Views.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels.Tools
{
    public class ToolBaseViewModel : ObservableObject
    {
        public ToolPageBase ToolPageBase;
        public void Init(ToolPageBase toolPageBase)
        {
            ToolPageBase = toolPageBase;
        }
        public void ShowMsg(string msg, bool autoClose = true)
        {
            ToolPageBase.ShowMsg(msg, autoClose);
        }
        public void ShowError(string msg, bool autoClose = true)
        {
            ToolPageBase.ShowError(msg, autoClose);
        }
        public void ShowSuccess(string msg, bool autoClose = true)
        {
            ToolPageBase.ShowSuccess(msg, autoClose);
        }
        public void ShowWaiting()
        {
            ToolPageBase.ShowWaiting();
        }
        public void HideWaiting()
        {
            ToolPageBase.HideWaiting();
        }
    }
}
