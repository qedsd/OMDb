using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class StorageCard : UserControl
    {
        public StorageCard()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty EnrtyStorageProperty = DependencyProperty.Register
            (
            "EnrtyStorage",
            typeof(EnrtyStorage),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetEnrtyStorage))
            );
        private static void SetEnrtyStorage(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as StorageCard;
            if (card != null)
            {
                card.EnrtyStorage = e.NewValue as EnrtyStorage;
                var es = card.EnrtyStorage;
                //if (es == null || string.IsNullOrEmpty(es.StoragePath))
                //{
                //    card.AddGrid.Visibility = Visibility.Visible;
                //    card.ShowGrid.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    card.AddGrid.Visibility = Visibility.Collapsed;
                //    card.ShowGrid.Visibility = Visibility.Visible;
                //}
            }
        }
        public EnrtyStorage EnrtyStorage
        {
            get { return (EnrtyStorage)GetValue(EnrtyStorageProperty); }

            set { SetValue(EnrtyStorageProperty, value); }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var file = await Dialogs.EditStorageDialog.ShowDialog();
            if (file != null && !string.IsNullOrEmpty(file.StoragePath))
            {
                AddStorageEvent?.Invoke(file);
            }
        }
        public delegate void AddStorage(Models.EnrtyStorage enrtyStorage);
        public static event AddStorage AddStorageEvent;

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            await Dialogs.EditStorageDialog.ShowDialog(EnrtyStorage);
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            if(await Dialogs.QueryDialog.ShowDialog("是否确认移除?","不会删除文件"))
            {
                RemoveStorageEvent?.Invoke(EnrtyStorage);
            }
        }


        public delegate void RemoveStorage(Models.EnrtyStorage enrtyStorage);
        public static event RemoveStorage RemoveStorageEvent;

    }
}
