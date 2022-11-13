using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Views.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class ToolsPage : Page
    {
        public ToolsPage()
        {
            this.InitializeComponent();
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            AddTabViewItem((sender as Button).Content, new SubToolPage());
        }

        private void AddTabViewItem(object header,object content)
        {
            TipTextBlock.Visibility = Visibility.Collapsed;
            TabViewItem tabViewItem = new TabViewItem();
            tabViewItem.Header = header;
            tabViewItem.Content = content;
            tabViewItem.CloseRequested += TabViewItem_CloseRequested;
            TabView.TabItems.Add(tabViewItem);
            TabView.SelectedItem = tabViewItem;
        }

        private void TabViewItem_CloseRequested(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
        {
            TabView.TabItems.Remove(sender);
            if(TabView.TabItems.Count == 0)
            {
                TipTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
