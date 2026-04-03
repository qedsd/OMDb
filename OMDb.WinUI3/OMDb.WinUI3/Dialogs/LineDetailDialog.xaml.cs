using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class LineDetailDialog : DialogBase
    {
        public LineDetailDialog(Core.Models.ExtractsLineBase line, string name)
        {
            //TODO:字体设置
            this.InitializeComponent();
            LineTextBlock.Text = line.Line;
            NameTextBlock.Text = name;
            FromTextBlock.Text = line.From;
            if(string.IsNullOrEmpty(FromTextBlock.Text))
            {
                FromTextBlock.Visibility = Visibility.Collapsed;
            }
            UpdateTimeTextBlock.Text = line.UpdateTime.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
