using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT;

namespace OMDb.WinUI3.Views
{
    public sealed partial class EntryCollectionDetailPage : Page,Interfaces.ITabViewItemPage
    {
        public string Title { get; private set; }
        public EntryCollectionDetailPage(EntryCollection entryCollection)
        {
            this.InitializeComponent();
            (DataContext as EntryCollectionDetailViewModel).EntryCollection = entryCollection;
            Title = entryCollection.Title;
        }

        private void EditFlyout_Button_Click(object sender, RoutedEventArgs e)
        {
            EditNameFlyout.Hide();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            //if(AdaptiveGridView.SelectedItems != null && AdaptiveGridView.SelectedItems.Count != 0)
            //{
            //    var list = AdaptiveGridView.SelectedItems.Select(p => p as Core.Models.EntryCollectionItem).ToList();
            //    (DataContext as EntryCollectionDetailViewModel).RemoveCommand.Execute(list);
            //}
            var item = (e.OriginalSource as FrameworkElement).DataContext as Core.Models.EntryCollectionItem;
            if(item != null)
            {
                (DataContext as EntryCollectionDetailViewModel).RemoveOneCommand.Execute(item);
            }
        }

        public void Close()
        {
            
        }
    }
}
