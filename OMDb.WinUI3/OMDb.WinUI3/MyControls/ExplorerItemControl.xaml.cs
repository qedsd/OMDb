using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class ExplorerItemControl : UserControl
    {
        public ExplorerItemControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register
            (
            "ItemsSource",
            typeof(IEnumerable<Models.ExplorerItem>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetItemsSource))
            );
        private static void SetItemsSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as ExplorerItemControl;
            if (card != null)
            {
                card.TreeView.ItemsSource = e.NewValue as IEnumerable<Models.ExplorerItem>;
            }
        }
        public IEnumerable<Models.ExplorerItem> ItemsSource
        {
            get { return (IEnumerable<Models.ExplorerItem>)GetValue(ItemsSourceProperty); }

            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// 视频文件右键打开事件
        /// TreeViewItem的command无法绑定，用click来手动触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExplorerItem_Click(object sender, RoutedEventArgs e)
        {
            OpenItem((sender as MenuFlyoutItem).DataContext as ExplorerItem);
        }

        private void ExplorerItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            OpenItem((sender as TreeViewItem).DataContext as ExplorerItem);
        }
        private void OpenItem(ExplorerItem explorerItem)
        {
            System.Diagnostics.Process.Start("explorer.exe", explorerItem.FullName);
        }
    }
}
