﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Services;
using OMDb.Core.Utils;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Dialogs;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services.Settings;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace OMDb.WinUI3.Views
{
    public sealed partial class LabelPropertyPage : Page
    {
        public ViewModels.LabelPropertyViewModel VM { get; set; } = new ViewModels.LabelPropertyViewModel();
        public LabelPropertyPage()
        {
            this.InitializeComponent();
            DataContext = VM;
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            LabelPropertyTree current_LPEZ = (LabelPropertyTree)e.AddedItems.FirstOrDefault();
            if (current_LPEZ == null)
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
                return;
            }
            var lst_LK = Core.Services.LabelPropertyService.GetLinkId(current_LPEZ.LabelProperty.LPDb.LPID);
            if (!(lst_LK.Count > 0))
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
                return;
            }
            SetLinkTab(lst_LK);
        }



        private void SetLinkTab(List<string> linkIdList)
        {
            this.TabView_LPEZ_Link.TabItems.Clear();
            VM.Current_LPEZ_Link.Clear();
            foreach (var lpbaba in VM.LabelPropertyTrees)
            {
                var lpRoot = new LabelPropertyTree(lpbaba.LabelProperty.LPDb);
                foreach (var lpez in lpbaba.Children)
                {
                    if (linkIdList.Contains(lpez.LabelProperty.LPDb.LPID))
                    {
                        var lpChild = new LabelPropertyTree(lpez.LabelProperty.LPDb);
                        lpRoot.Children.Add(lpChild);
                    }
                }
                if (lpRoot.Children.Count > 0)
                {
                    VM.Current_LPEZ_Link.Add(lpRoot);
                }
            }
            foreach (var lpbaba in VM.Current_LPEZ_Link)
            {
                var tbi = new TabViewItem();
                tbi.Header = lpbaba.LabelProperty.LPDb.Name;
                var lv = new ListView();
                foreach (var lpez in lpbaba.Children)
                {
                    var lvi = new ListViewItem();
                    lvi.Content = lpez.LabelProperty.LPDb.Name;
                    lv.Items.Add(lvi);
                }
                tbi.Content = lv;
                this.TabView_LPEZ_Link.TabItems.Add(tbi);
            }
            if (VM.Current_LPEZ_Link.Count() > 0)
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Visible;
                this.TabView_LPEZ_Link.SelectedIndex = 0;
            }
            else
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
            }
        }

        #region 标题右键

        //添加数据
        private async void ADD_PropertyData_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog();
            //取消
            if (result == null)
                return;
            //确认 -> 标签名为空
            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }
            //确认 -> 数据库创建标签
            else
            {
                result.Level = 2;
                result.ParentId = item.LPDb.LPID;
                LabelPropertyService.AddLabelProperty(result);
                await this.VM.InitAsync();
                Helpers.InfoHelper.ShowSuccess($"添加属性数据<{result.Name}>成功！");
            }
        }
        //编辑标题
        private async void Edit_Property_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog(item.LPDb);
            
            if (result == null)//取消
                return;
            //确认 -> 标签名为空
            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }
            //确认 -> 更新数据库
            else
            {
                LabelPropertyService.UpdateLabel(result);
                await this.VM.InitAsync();
                Helpers.InfoHelper.ShowSuccess($"<{item.LPDb.Name}>编辑成功");
            }
        }
        //删除标题
        private async void Delete_Property_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            var itemTree = this.VM.LabelPropertyTrees.FirstOrDefault(a => a.LabelProperty.LPDb.LPID == item.LPDb.LPID);
            var data = itemTree.Children.Select(a => a.LabelProperty.LPDb.LPID).ToList();
            data.Add(item.LPDb.LPID);
            LabelPropertyService.RemoveLabel(data);
            this.VM.LabelPropertyTrees.Remove(itemTree);
            await this.VM.InitAsync();
            Helpers.InfoHelper.ShowSuccess($"<{item.LPDb.Name}>删除成功！");
        }
        //标题关联
        private async void Edit_PropertyLink_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            var result = await EditLabelPropertyLinkDialog.ShowDialog(item);
            if (result == null)
                return;
            var linkIdListOld = Core.Services.LabelPropertyService.GetLinkId(item.LPDb.LPID);
            var linkIdListDelete = linkIdListOld.Where(a => !(result.Contains(a))).ToList();
            var linkIdListNew = result.Where(a => !(linkIdListOld.Contains(a))).ToList();
            Core.Services.LabelPropertyService.RemoveLabelPropertyLink(item.LPDb.LPID, linkIdListDelete);
            Core.Services.LabelPropertyService.AddLabelPropertyLink(item.LPDb.LPID, linkIdListNew);
            await this.VM.InitAsync();
            var linkIdList = Core.Services.LabelPropertyService.GetLinkId(item.LPDb.LPID);
            this.SetLinkTab(linkIdList);
            Helpers.InfoHelper.ShowSuccess($"<{item.LPDb.Name}>关联成功");

        }
        #endregion

        #region 数据右键
        private async void Edit_PropertyDataLink_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            var result = await EditLabelPropertyDataLinkDialog.ShowDialog(item.LPDb);
            if (result == null)
                return;
            Core.Services.LabelPropertyService.ClearLabelPropertyLink(item.LPDb.LPID);
            Core.Services.LabelPropertyService.AddLabelPropertyLink(item.LPDb.LPID, result);
            await VM.InitAsync();
            var linkIdList = Core.Services.LabelPropertyService.GetLinkId(item.LPDb.LPID);
            SetLinkTab(linkIdList);
            Helpers.InfoHelper.ShowSuccess($"<{item.LPDb.Name}>关联成功");
        }
        private async void Edit_PropertyData_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog(item.LPDb);

            if (result == null)//取消
                return;

            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }//确认 -> 标签名为空
            LabelPropertyService.UpdateLabel(result);//更新数据库
            this.VM.RefreshCommandWithParam.Execute(item.LPDb.ParentId);
            await VM.InitAsync();
            Helpers.InfoHelper.ShowSuccess($"<{item.LPDb.Name}>编辑成功");
        }
        private async void Delete_PropertyData_Click_Right(object sender, RoutedEventArgs e)
        {
            var item = ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty;
            LabelPropertyService.RemoveLabel(item.LPDb.LPID);
            this.VM.RefreshCommandWithParam.Execute(item.LPDb.ParentId);
            await VM.InitAsync();
            Helpers.InfoHelper.ShowSuccess($"<{item.LPDb.Name}>删除成功！");
        }
        #endregion


        #region 弃用
        private async void ADD_Property_Click(object sender, RoutedEventArgs e)
        {
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog();
            //取消
            if (result == null)
                return;
            //确认 -> 标签名为空
            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }
            //确认 -> 数据库创建标签
            else
            {
                result.Level = 1;
                LabelPropertyService.AddLabelProperty(result);
                VM.InitAsync();
                Helpers.InfoHelper.ShowSuccess($"添加属性“{result.Name}”成功！");
            }
        }
        private async void ADD_PropertyData_Click(object sender, RoutedEventArgs e)
        {
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog();
            //取消
            if (result == null)
                return;
            //确认 -> 标签名为空
            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }
            //确认 -> 数据库创建标签
            else
            {
                result.Level = 2;
                var currentRoot = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
                result.ParentId = currentRoot.LabelProperty.LPDb.LPID;
                LabelPropertyService.AddLabelProperty(result);
                VM.InitAsync();
                Helpers.InfoHelper.ShowSuccess($"添加属性数据“{result.Name}”成功！");
            }
        }

        private async void ADD_PropertyDataLink_Click(object sender, RoutedEventArgs e)
        {
            var item = (LabelPropertyTree)this.GridView_Current_LPEZCollection.SelectedItem;
            if (item == null)
            {
                Helpers.InfoHelper.ShowMsg("请选择属性数据！");
                return;
            }
            var result = await EditLabelPropertyDataLinkDialog.ShowDialog(item.LabelProperty.LPDb);
            if (result == null || !(result.Count > 0))
            {
                return;
            }
            else
            {
                Core.Services.LabelPropertyService.AddLabelPropertyLink(item.LabelProperty.LPDb.LPID, result);
                VM.InitAsync();
                var lst_LK = Core.Services.LabelPropertyService.GetLinkId(item.LabelProperty.LPDb.LPID);
                SetLinkTab(lst_LK);
                Helpers.InfoHelper.ShowSuccess($"“{item.LabelProperty.LPDb.Name}”添加关联成功");
            }
        }
        private void Delete_Property_Click(object sender, RoutedEventArgs e)
        {
            var item = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            if (item == null)
            {
                Helpers.InfoHelper.ShowMsg("请选择属性！");
                return;
            }
            var lstStr = item.Children.Select(x => x.LabelProperty.LPDb.LPID).ToList();
            lstStr.Add(item.LabelProperty.LPDb.LPID);
            LabelPropertyService.RemoveLabel(lstStr);
            VM.InitAsync();
            Helpers.InfoHelper.ShowSuccess($"删除属性“{item.LabelProperty.LPDb.Name}”成功！");
        }
        private void Delete_PropertyData_Click(object sender, RoutedEventArgs e)
        {
            var item = (LabelPropertyTree)this.GridView_Current_LPEZCollection.SelectedItem;
            if (item == null)
            {
                Helpers.InfoHelper.ShowMsg("请选择属性数据！");
                return;
            }
            LabelPropertyService.RemoveLabel(item.LabelProperty.LPDb.LPID);
            VM.InitAsync();
            Helpers.InfoHelper.ShowSuccess($"删除属性数据“{item.LabelProperty.LPDb.Name}”成功！");
        }
        private void Delete_PropertyLink_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Delete_PropertyDataLink_Click(object sender, RoutedEventArgs e)
        {

            var item = (LabelPropertyTree)this.GridView_Current_LPEZCollection.SelectedItem;
            if (item == null)
            {
                Helpers.InfoHelper.ShowMsg("请选择属性数据！");
                return;
            }
            LabelPropertyService.ClearLabelPropertyLink(item.LabelProperty.LPDb.LPID);
            this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
        }
        private async void Edit_Property_Click(object sender, RoutedEventArgs e)
        {
            var item = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            if (item == null)
            {
                Helpers.InfoHelper.ShowMsg("请选择属性！");
                return;
            }
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog(item.LabelProperty.LPDb);
            //取消
            if (result == null)
                return;
            //确认 -> 标签名为空
            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }
            //确认 -> 更新数据库
            else
            {
                LabelPropertyService.UpdateLabel(result);
                VM.InitAsync();
                Helpers.InfoHelper.ShowSuccess($"“{item.LabelProperty.LPDb.Name}”编辑成功");
            }
        }
        private async void Edit_PropertyData_Click(object sender, RoutedEventArgs e)
        {
            var item = (LabelPropertyTree)this.GridView_Current_LPEZCollection.SelectedItem;
            if (item == null)
            {
                Helpers.InfoHelper.ShowMsg("请选择属性数据！");
                return;
            }
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog(item.LabelProperty.LPDb);
            //取消
            if (result == null)
                return;
            //确认 -> 标签名为空
            if (result.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                Helpers.InfoHelper.ShowMsg("标签名称不能为空！");
                return;
            }
            //确认 -> 更新数据库
            else
            {
                LabelPropertyService.UpdateLabel(result);
                VM.InitAsync();
                Helpers.InfoHelper.ShowSuccess($"“{item.LabelProperty.LPDb.Name}”编辑成功");
            }
        }



        #endregion


    }
}
