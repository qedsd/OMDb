using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.ViewModels.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views.Tools
{
    public sealed partial class AVCodecToolPage : ToolPageBase
    {
        public AVCodecToolPage(string toolName) : base(toolName)
        {
            this.InitializeComponent();
            (DataContext as ToolBaseViewModel).Init(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as AVCodecToolViewModel).PickSaveFileCommand.Execute((sender as FrameworkElement).DataContext);
        }
    }
}
