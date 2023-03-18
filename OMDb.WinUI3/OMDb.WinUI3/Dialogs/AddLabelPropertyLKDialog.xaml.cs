﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.DbModels;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddLabelPropertyLKDialog : Page
    {
        public ViewModels.LabelPropertyViewModel VM { get; set; } = new ViewModels.LabelPropertyViewModel();
        public AddLabelPropertyLKDialog()
        {
            this.InitializeComponent();

        }

        public static async Task<string> ShowDialog()
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "关联标签选择";
            dialog.PrimaryButton.Content = "确认";
            dialog.CancelButton.Content = "取消";
            AddLabelPropertyLKDialog content = new AddLabelPropertyLKDialog();
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                //数据库新增属性&属性数据 合并返回
                var ab=content.Link_Table.ItemsSource;
                return null;
            }
            else
            {
                return null;
            }
        }

        private void ImportLink_Click(object sender, RoutedEventArgs e)
        {
            var items = this.GridView_Current_LPEZCollection.SelectedItems;
            var itemsRoot = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            foreach (var item in items)
            {
                var lpt=item as LabelPropertyTree;
                VM.DtData.Add(lpt.LPDb);
            }
        }

        private void BroomLink_Click(object sender, RoutedEventArgs e)
        {
            VM.DtData.Clear();
        }

        private void AddLink_Click(object sender, RoutedEventArgs e)
        {
            var lp = new LabelPropertyDb();
            var itemsRoot = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            lp.ParentId = itemsRoot.LPDb.LPId;
            VM.DtData.Add(lp);
        }

        private void RemoveLink_Click(object sender, RoutedEventArgs e)
        {
            var items = this.Link_Table.SelectedItems.ToList();
            foreach (var item in items)
            {
                var lp = (LabelPropertyDb)item;
                var result=VM.DtData.Remove(lp);
            }
        }
    }
}