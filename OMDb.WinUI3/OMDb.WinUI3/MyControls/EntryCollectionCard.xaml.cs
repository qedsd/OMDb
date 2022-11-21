using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
    internal sealed partial class EntryCollectionCard : UserControl
    {
        public EntryCollectionCard()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty EntryCollectionProperty = DependencyProperty.Register(
           "EntryCollection", typeof(EntryCollection), typeof(EntryCollectionCard), new PropertyMetadata(null, new PropertyChangedCallback(OnEntryCollectionChanged)));
        private static void OnEntryCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as EntryCollectionCard;
            var collection = e.NewValue as EntryCollection;
            card.TitleTextBlock.Text = collection.Title;
            card.DescTextBlock.Text = collection.Description;
            card.LastUpdateTextBlock.Text = collection.LastUpdateTime.ToString();
            card.WatchedCountTextBlock.Text = collection.WatchedCount.ToString();
            card.TotalCountTextBlock.Text = collection.Items == null? "0" : collection.Items.Count.ToString();
            if(collection.CoverImage != null)
            {
                card.CoverImage.Source = collection.CoverImage;

            }
            else
            {
                card.CoverImage.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")));
            }
        }

        public EntryCollection EntryCollection
        {
            get { return (EntryCollection)GetValue(EntryCollectionProperty); }

            set { SetValue(EntryCollectionProperty, value); }
        }
    }
}
