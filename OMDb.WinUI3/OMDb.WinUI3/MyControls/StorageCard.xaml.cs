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
        //#region 词条数
        //public static readonly DependencyProperty EntryCountProperty = DependencyProperty.Register
        //    (
        //    "EntryCount",
        //    typeof(int),
        //    typeof(UserControl),
        //    new PropertyMetadata(0, new PropertyChangedCallback(SetEntryCount))
        //    );
        //private static void SetEntryCount(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var card = d as StorageCard;
        //    if (card != null)
        //    {
        //        card.TextBlock_EntryCount.Text = e.NewValue.ToString();
        //    }
        //}
        //public int EntryCount
        //{
        //    get { return (int)GetValue(EntryCountProperty); }

        //    set { SetValue(EntryCountProperty, value); }
        //}
        //#endregion
        //#region 仓库名
        //public static readonly DependencyProperty StorageNameProperty = DependencyProperty.Register
        //    (
        //    "StorageName",
        //    typeof(string),
        //    typeof(UserControl),
        //    new PropertyMetadata(0, new PropertyChangedCallback(SetStorageName))
        //    );
        //private static void SetStorageName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var card = d as StorageCard;
        //    if (card != null)
        //    {
        //        card.TextBlock_StorageName.Text = e.NewValue.ToString();
        //    }
        //}
        //public string StorageName
        //{
        //    get { return (string)GetValue(StorageNameProperty); }

        //    set { SetValue(StorageNameProperty, value); }
        //}
        //#endregion
        //#region 仓库路径
        //public static readonly DependencyProperty StoragePathProperty = DependencyProperty.Register
        //    (
        //    "StoragePath",
        //    typeof(string),
        //    typeof(UserControl),
        //    new PropertyMetadata(0, new PropertyChangedCallback(SetStoragePath))
        //    );
        //private static void SetStoragePath(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var card = d as StorageCard;
        //    if (card != null)
        //    {
        //        if (string.IsNullOrEmpty(e.NewValue.ToString()))//空，显示添加按钮
        //        {
        //            card.AddGrid.Visibility = Visibility.Visible;
        //            card.ShowGrid.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            card.AddGrid.Visibility = Visibility.Collapsed;
        //            card.ShowGrid.Visibility = Visibility.Visible;
        //            card.TextBlock_StoragePath.Text = e.NewValue.ToString();
        //        }
        //    }
        //}
        //public string StoragePath
        //{
        //    get { return (string)GetValue(StoragePathProperty); }

        //    set { SetValue(StoragePathProperty, value); }
        //}
        //#endregion
        //#region 封面图片
        //public static readonly DependencyProperty CoverImgProperty = DependencyProperty.Register
        //    (
        //    "CoverImg",
        //    typeof(string),
        //    typeof(UserControl),
        //    new PropertyMetadata(0, new PropertyChangedCallback(SetCoverImg))
        //    );
        //private static void SetCoverImg(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var card = d as StorageCard;
        //    if (card != null)
        //    {
        //        if (!string.IsNullOrEmpty(e.NewValue.ToString()))
        //        {
        //            card.Image_Cover.Source = new BitmapImage(new Uri(e.NewValue.ToString()));
        //        }
        //    }
        //}
        //public string CoverImg
        //{
        //    get { return (string)GetValue(CoverImgProperty); }

        //    set { SetValue(CoverImgProperty, value); }
        //}
        //#endregion
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
                if(card.EnrtyStorage == null)
                {
                    card.AddGrid.Visibility = Visibility.Visible;
                    card.ShowGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (string.IsNullOrEmpty(card.EnrtyStorage.StoragePath))
                    {
                        card.AddGrid.Visibility = Visibility.Visible;
                        card.ShowGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        card.AddGrid.Visibility = Visibility.Collapsed;
                        card.ShowGrid.Visibility = Visibility.Visible;
                    }
                }
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
