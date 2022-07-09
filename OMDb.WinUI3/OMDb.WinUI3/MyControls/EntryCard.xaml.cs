﻿using Microsoft.UI.Xaml;
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
        private static void SetEnrty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as EntryCard;
            if (card != null)
            {
                card.Entry = e.NewValue as Entry;
                if(card.Entry != null)
                {
                    card.Image_Cover.Source = new BitmapImage(new Uri(System.IO.Path.Combine(card.Entry.Path, card.Entry.CoverImg)));
                    card.TextBlock_Name.Text = card.Entry.Name;
                }
            }
        }
        public Entry Entry
        {
            get { return (Entry)GetValue(EntryProperty); }

            set { SetValue(EntryProperty, value); }
        }
    }
}
