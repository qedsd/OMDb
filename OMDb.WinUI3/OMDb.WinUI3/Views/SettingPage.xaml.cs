﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class SettingPage : Page
    {
        public ViewModels.SettingViewModel VM { get; set; } = new ViewModels.SettingViewModel();
        public SettingPage()
        {
            this.InitializeComponent();
        }

        private void ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var item = e.Items.First() as Models.HomeItemConfig;
            e.Data.SetText(item.Name);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private async void ListView_Drop(object sender, DragEventArgs e)
        {
            ListView target = (ListView)sender;
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                DragOperationDeferral def = e.GetDeferral();
                string itemName = await e.DataView.GetTextAsync();
                Models.HomeItemConfig homeItem = VM.ActiveHomeItems.FirstOrDefault(p => p.Name == itemName);
                if (homeItem == null)
                {
                    homeItem = VM.InactiveHomeItems.FirstOrDefault(p => p.Name == itemName);
                }
                Windows.Foundation.Point pos = e.GetPosition(target.ItemsPanelRoot);

                // If the target ListView has items in it, use the height of the first item
                //      to find the insertion index.
                int index = 0;
                if (target.Items.Count != 0)
                {
                    // Get a reference to the first item in the ListView
                    ListViewItem sampleItem = (ListViewItem)target.ContainerFromIndex(0);

                    // Adjust itemHeight for margins
                    double itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

                    // Find index based on dividing number of items by height of each item
                    index = Math.Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

                    // Find the item being dropped on top of.
                    ListViewItem targetItem = (ListViewItem)target.ContainerFromIndex(index);

                    // If the drop position is more than half-way down the item being dropped on
                    //      top of, increment the insertion index so the dropped item is inserted
                    //      below instead of above the item being dropped on top of.
                    Windows.Foundation.Point positionInItem = e.GetPosition(targetItem);
                    if (positionInItem.Y > itemHeight / 2)
                    {
                        index++;
                    }

                    // Don't go out of bounds
                    index = Math.Min(target.Items.Count, index);
                }
                // Only other case is if the target ListView has no items (the dropped item will be
                //      the first). In that case, the insertion index will remain zero.
                // Find correct source list
                if (target.Name == "ActiveListView")
                {
                    // Find the ItemsSource for the target ListView and insert
                    VM.ActiveHomeItems.Insert(index, homeItem);
                    //Go through source list and remove the items that are being moved
                    VM.InactiveHomeItems.Remove(homeItem);
                }
                else if (target.Name == "InactiveListView")
                {
                    // Find the ItemsSource for the target ListView and insert
                    VM.InactiveHomeItems.Insert(index, homeItem);
                    //Go through source list and remove the items that are being moved
                    VM.ActiveHomeItems.Remove(homeItem);
                }
            }
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            // We don't want to show the Move icon
            e.DragUIOverride.IsGlyphVisible = false;
        }


        //删除数据中心点击事件
        private async void RadioButtonDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var flag = await Dialogs.QueryDialog.ShowDialog("再次确认", "请确认是否删除");
            if (flag)
            {
                var dbId = ((OMDb.WinUI3.Models.DbCenter)this.RadioButtonss.SelectedItem).DbCenterDb.Id; 
                Services.Settings.DbSelectorService.RemoveDbAsync(dbId);
                VM.DbSelector_Refresh.Execute(null);
                Helpers.InfoHelper.ShowSuccess("删除完成");
            }
            else
            {
                return;
            }
        }

        //编辑数据中心点击事件
        private async void RadioButtonEditButton_Click(object sender, RoutedEventArgs e)
        {
            var dbName = ((OMDb.WinUI3.Models.DbCenter)this.RadioButtonss.SelectedItem).DbCenterDb.DbName;
            //var dbName = ((OMDb.WinUI3.Models.DbCenter)((Microsoft.UI.Xaml.Controls.Primitives.ButtonBase)e.OriginalSource).CommandParameter).DbCenterDb.DbName;
            var dbName_New = await Dialogs.EditDbCenter.ShowDialog(dbName);

            if (dbName_New == "04833378-22bb-465b-9582-fb1bab622de")
            {
                VM.DbSelector_Refresh.Execute(null);
                return;
            }
            else if (dbName_New == null || dbName_New.Count() == 0)
            {
                Helpers.InfoHelper.ShowError("请输入DbName");
            }
            else if (dbName_New==dbName)
            {
                VM.DbSelector_Refresh.Execute(null);
                return;                
            }
            else if (Services.Settings.DbSelectorService.dbsCollection.Select(a => a.DbCenterDb.DbName).ToList().Contains(dbName_New))
            {
                Helpers.InfoHelper.ShowError("已存在同名DbCenter");
            }
            else
            {
                ((OMDb.WinUI3.Models.DbCenter)this.RadioButtonss.SelectedItem).DbCenterDb.DbName = dbName_New;
                Services.Settings.DbSelectorService.EditDbAsync(((OMDb.WinUI3.Models.DbCenter)this.RadioButtonss.SelectedItem).DbCenterDb);
                Helpers.InfoHelper.ShowSuccess("编辑完成");
                VM.DbSelector_Refresh.Execute(null);
            }
        }
    }
}
