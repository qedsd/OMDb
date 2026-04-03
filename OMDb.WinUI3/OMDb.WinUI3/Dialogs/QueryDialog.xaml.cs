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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class QueryDialog : DialogBase
    {
        public QueryDialog(string title,string content)
        {
            this.InitializeComponent();
            TitleTextBlock.Text = title;
            TextBlock_Query.Text = content;
        }
        private bool clickConfirm = false;

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            clickConfirm = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            clickConfirm = true;
            Close();
        }

        public override async Task<bool> ShowAsync()
        {
            await base.ShowAsync();
            return clickConfirm;
        }
        public static async Task<bool> ShowDialog(string title, string content)
        {
            return await Helpers.InfoHelper.ShowQueryAsync(title, content);
        }
    }
}
