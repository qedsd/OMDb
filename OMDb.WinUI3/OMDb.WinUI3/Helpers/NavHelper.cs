using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace OMDb.WinUI3.Helpers
{
    public class NavHelper
    {
        // This helper class allows to specify the page that will be shown when you click on a NavigationViewItem
        //
        // Usage in xaml:
        // <winui:NavigationViewItem x:Uid="Shell_Main" Icon="Document" helpers:NavHelper.NavigateTo="views:MainPage" />
        //
        // Usage in code:
        // NavHelper.SetNavigateTo(navigationViewItem, typeof(MainPage));

        //public static void SetNavigateTo(Button item, Type value)
        //{
        //    item.SetValue(NavigateToProperty, value);
        //}
        //public static Type GetNavigateTo(Button item)
        //{
        //    return (Type)item.GetValue(NavigateToProperty);
        //}
        public static void SetNavigateTo(ListViewItem item, Type value)
        {
            item.SetValue(NavigateToProperty, value);
        }

        
        public static Type GetNavigateTo(ListViewItem item)
        {
            return (Type)item.GetValue(NavigateToProperty);
        }
        public static readonly DependencyProperty NavigateToProperty =
            DependencyProperty.RegisterAttached("NavigateTo", typeof(Type), typeof(NavHelper), new PropertyMetadata(null));
    }
}
