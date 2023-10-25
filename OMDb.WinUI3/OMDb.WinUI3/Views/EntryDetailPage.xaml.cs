using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class EntryDetailPage : Page, ITabViewItemPage
    {
        public ViewModels.EntryDetailViewModel VM { get; set; } = new ViewModels.EntryDetailViewModel(null);

        public string Title { get; private set; }

        public EntryDetailPage(Core.Models.Entry entry)
        {
            this.InitializeComponent();
            Init(entry);
        }
        private async void Init(Core.Models.Entry entry)
        {
            if (entry != null)
            {
                VM.Entry = await Models.EntryDetail.CreateAsync(entry);
                Title = VM.Entry.Name;
            }
        }

        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", (sender as Grid).DataContext as string);
        }

        private void EditNameFlyout_Button_Click(object sender, RoutedEventArgs e)
        {
            EditNameFlyout.Hide();
        }

        private void PrevImg_Click(object sender, RoutedEventArgs e)
        {
            ImgScrollViewer.ScrollToHorizontalOffset(ImgScrollViewer.HorizontalOffset - 300);
        }

        private void NextImg_Click(object sender, RoutedEventArgs e)
        {
            ImgScrollViewer.ScrollToHorizontalOffset(ImgScrollViewer.HorizontalOffset+300);
        }


        private void LineListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.LineDetailCommand.Execute(e.ClickedItem);
        }

        private void LineListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            VM.LineDetailCommand.Execute((sender as ListView).SelectedItem);
        }

        public void Close()
        {
            
        }
    }
}
