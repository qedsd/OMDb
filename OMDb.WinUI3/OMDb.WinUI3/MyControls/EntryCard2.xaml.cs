using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
    public sealed partial class EntryCard2 : UserControl
    {
        public EntryCard2()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty EntryProperty = DependencyProperty.Register
            (
            "EntryCollectionItem",
            typeof(EntryCollectionItem),
            typeof(UserControl),
            new PropertyMetadata(null)
            );
        public EntryCollectionItem EntryCollectionItem
        {
            get { return (EntryCollectionItem)GetValue(EntryProperty); }

            set { SetValue(EntryProperty, value); }
        }

        private void Item_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Storyboard1.Begin();
        }

        private void Item_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Storyboard2.Begin();
        }
    }
}
