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
        }

        public static void ReomveItem(TabViewItem item)
        {
            (item.Content as ITabViewItemPage)?.Close();
            TabView.TabItems.Remove(item);
        }
    }
}
