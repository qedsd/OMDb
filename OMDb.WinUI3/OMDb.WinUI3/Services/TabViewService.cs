using Microsoft.UI.Xaml.Controls;
using OMDb.WinUI3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class TabViewService
    {
        private static TabView TabView { get; set; }
        public static void Init(TabView tabView)
        {
            TabView = tabView;
            TabView.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        public static TabView GetTabView()
        {
            return TabView;
        }
        public static void AddItem(ITabViewItemPage itemPage)
        {
            TabViewItem item = new TabViewItem()
            {
                Header = itemPage.Title,
                IsSelected = true,
                Content = itemPage
            };
            TabView.TabItems.Add(item);
            SetTabViewVisibility();
        }

        public static void ReomveItem(TabViewItem item)
        {
            (item.Content as ITabViewItemPage)?.Close();
            TabView.TabItems.Remove(item);
            SetTabViewVisibility();
        }

        private static void SetTabViewVisibility()
        {
            if(TabView.TabItems.Any())
            {
                TabView.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
            else
            {
                TabView.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
