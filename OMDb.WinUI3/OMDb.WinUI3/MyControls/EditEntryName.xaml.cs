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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class EditEntryName : UserControl
    {
        public EditEntryName()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register
            (
            "ItemSource",
            typeof(ObservableCollection<EntryName>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetItemSource))
            );
        private static void SetItemSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EditEntryName;
            if (control != null)
            {
                var names = e.NewValue as ObservableCollection<EntryName>;
                if (names != null)
                {
                    control.NamesListBox.ItemsSource = names;
                }
            }
        }
        public ObservableCollection<EntryName> ItemSource
        {
            get { return (ObservableCollection<EntryName>)GetValue(ItemSourceProperty); }

            set { SetValue(ItemSourceProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemSource.Add(new EntryName());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ItemSource.Remove((sender as Button).DataContext as EntryName);
        }
    }
}
