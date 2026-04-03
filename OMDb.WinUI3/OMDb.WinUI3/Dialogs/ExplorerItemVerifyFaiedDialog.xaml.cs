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
    public sealed partial class ExplorerItemVerifyFaiedDialog : Page
    {
        public ExplorerItemVerifyFaiedDialog(List<Models.ExplorerItem> explorerItems)
        {
            this.InitializeComponent();
            ListBox.ItemsSource = explorerItems;
        }
        private static string EntryPath;
        public static async Task ShowDialog(List<Models.ExplorerItem> explorerItems,string entryPath)
        {
            EntryPath = entryPath;
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "校验失败文件";
            dialog.CancelButton.Content = "关闭";
            dialog.PrimaryButton.Content = "打开词条文件夹";
            dialog.PrimaryButton.Click += PrimaryButton_Click;
            dialog.ContentFrame.Content = new ExplorerItemVerifyFaiedDialog(explorerItems);
            await dialog.ShowAsync();
        }

        private static void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.PathHelper.OpenBySystem(EntryPath);
        }

        /// <summary>
        /// 打开所在文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExplorerItem_Click(object sender, RoutedEventArgs e)
        {
            Helpers.PathHelper.OpenBySystem(((sender as FrameworkElement).DataContext as Models.ExplorerItem).FullName);
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Helpers.FileHelper.OpenBySystem(((sender as FrameworkElement).DataContext as Models.ExplorerItem).FullName);
        }
    }
}
