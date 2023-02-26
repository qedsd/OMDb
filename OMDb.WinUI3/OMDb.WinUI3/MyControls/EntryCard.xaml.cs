using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class EntryCard : UserControl
    {
        public EntryCard()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty EntryProperty = DependencyProperty.Register
            (
            "Entry",
            typeof(Entry),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetEnrty))
            );
        private static async void SetEnrty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as EntryCard;
            if (card != null)
            {
                card.Entry = e.NewValue as Entry;
                if(card.Entry != null)
                {
                    var samllStream = await Core.Helpers.ImageHelper.ResetSizeAsync(Services.PathService.EntryCoverImgFullPath(card.Entry), 400, 0);
                    card.Image_Cover.Source = await Helpers.ImgHelper.CreateBitmapImageAsync(samllStream);
                    card.TextBlock_Name.Text = card.Entry.Name;
                    card.TextBlock_Date.Text = card.Entry.ReleaseDate.HasValue? card.Entry.ReleaseDate.Value.Year.ToString():string.Empty;
                }
            }
        }
        public Entry Entry
        {
            get { return (Entry)GetValue(EntryProperty); }

            set { SetValue(EntryProperty, value); }
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await Services.EntryService.EditEntryAsync(Entry);
        }

        private async void MenuFlyoutItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            await Services.EntryService.RemoveEntryAsync(Entry);
        }
    }
}
