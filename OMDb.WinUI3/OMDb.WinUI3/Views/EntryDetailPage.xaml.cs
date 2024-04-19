using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Services.PluginsService;
using OMDb.Core.Utils.Extensions;
using OMDb.Core.Utils.PathUtils;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class EntryDetailPage : Page, ITabViewItemPage
    {
        public ViewModels.EntryDetailViewModel VM { get; set; } = new ViewModels.EntryDetailViewModel(null);

        public string Title { get; private set; }

        public EntryDetailPage(Core.Models.Entry entry)
        {
            this.InitializeComponent();
            Init(entry);
            InitGetEntryInfoDescriptionService();

        }
        private async void Init(Core.Models.Entry entry)
        {
            if (entry != null)
            {
                VM.Entry = await Models.EntryDetail.CreateAsync(entry);
                Title = VM.Entry.Name;
            }
        }

        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", (sender as Grid).DataContext as string);
        }

        private void EditNameFlyout_Button_Click(object sender, RoutedEventArgs e)
        {
            EditNameFlyout.Hide();
        }

        private void PrevImg_Click(object sender, RoutedEventArgs e)
        {
            ImgScrollViewer.ScrollToHorizontalOffset(ImgScrollViewer.HorizontalOffset - 300);
        }

        private void NextImg_Click(object sender, RoutedEventArgs e)
        {
            ImgScrollViewer.ScrollToHorizontalOffset(ImgScrollViewer.HorizontalOffset + 300);
        }


        private void LineListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.LineDetailCommand.Execute(e.ClickedItem);
        }

        private void LineListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            VM.LineDetailCommand.Execute((sender as ListView).SelectedItem);
        }

        public void Close()
        {

        }

        private async void GetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(this.ddb.Content).Equals("无服务")) return;
            this.pr.IsActive = true;
            this.btn_GetInfo.IsEnabled = false;
            var desc = await EntryInfoService.GetEntryInfoDescriptionNet(this.VM.Entry.Name, Convert.ToString(this.ddb.Content));
            this.pr.IsActive = false;
            this.btn_GetInfo.IsEnabled = true;

            #region 基本信息

            #endregion 
            if (!desc.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                this.VM.Desc= desc;
            }

        }

        /// <summary>
        /// 动态加载获取媒体信息服务
        /// </summary>
        private void InitGetEntryInfoDescriptionService()
        {
            #region 动态加载获取媒体信息服务
            if (PluginsBaseService.EntryInfoDescriptionExports.Count() > 0)
            {
                this.ddb.Content = PluginsBaseService.EntryInfoDescriptionExports.FirstOrDefault().GetType().Assembly.GetName().Name;
                var mf = new MenuFlyout();
                foreach (var item in PluginsBaseService.EntryInfoDescriptionExports)
                {
                    MenuFlyoutItem mfl = new MenuFlyoutItem();
                    mfl.Text = item.GetType().Assembly.GetName().Name;
                    mfl.Click += (s, e) => { this.ddb.Content = mfl.Text; };
                    mf.Items.Add(mfl);
                }
                this.ddb.Flyout = mf;
            }
            else this.ddb.Content = "无服务";
            #endregion
        }
    }
}
