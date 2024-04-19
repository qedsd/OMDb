using CommunityToolkit.WinUI.UI;
using ICSharpCode.SharpZipLib.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Services.PluginsService;
using OMDb.Core.Utils;
using OMDb.Core.Utils.Extensions;
using OMDb.Core.Utils.PathUtils;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditEntryDialog : Page
    {
        public ViewModels.EditEntryHomeViewModel VM { get; set; }
        public EditEntryDialog(Core.Models.Entry entry)
        {
            VM = new ViewModels.EditEntryHomeViewModel(entry);
            this.InitializeComponent();


            #region 动态加载获取媒体信息服务
            if (PluginsBaseService.EntryInfoExports.Count() > 0)
            {
                this.ddb.Content = PluginsBaseService.EntryInfoExports.FirstOrDefault().GetType().Assembly.GetName().Name;
                var mf = new MenuFlyout();
                foreach (var item in PluginsBaseService.EntryInfoExports)
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

            //封面不为空 -> 设置封面
            if (entry != null && entry.CoverImg != null)
                Image_CoverImg.Source = new BitmapImage(new Uri(VM.Entry.CoverImg));
        }



        /// <summary>
        /// 返回的entry的Path、CoverImg都为全路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<Models.EntryDetail> ShowDialog(Core.Models.Entry entry = null)
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = entry == null ? "新建词条" : "编辑词条";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            EditEntryDialog content = new EditEntryDialog(entry);
            dialog.ContentFrame.Content = content;

            //保存
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                //新增 or 編輯
                if (entry == null)
                {
                    entry = content.VM.Entry;
                    entry.Name = content.VM.EntryName;
                    entry.DbId = content.VM.SelectedEnrtyStorage?.Name;
                }
                else
                {
                    entry.CopyFrom(content.VM.Entry);
                    entry.Name = content.VM.EntryName;
                }
                var entryDetail = new Models.EntryDetail()
                {
                    Entry = entry,
                    Labels = content.VM.Labels.Select(p => p.LabelClassDb).ToList(),
                    FullEntryPath = content.VM.Entry.Path,
                };

                #region 封面处理
                //封面空
                if (content.VM.Entry.CoverImg == null || content.VM.Entry.CoverImg.Length <= 0)
                {
                    entryDetail.FullCoverImgPath = Services.CommonService.GetCover();
                }
                else
                {
                    entryDetail.FullCoverImgPath = content.VM.Entry.CoverImg;
                }
                #endregion

                return entryDetail;
            }

            //取消
            else
            {
                return null;
            }
        }







        //選擇路徑
        private async void Button_Path_Click(object sender, RoutedEventArgs e)
        {
            var folder = await Helpers.PickHelper.PickFolderAsync();
            if (folder != null)
            {
                if (!folder.Path.StartsWith(VM.SelectedEnrtyStorage.Path, StringComparison.OrdinalIgnoreCase))
                    Helpers.DialogHelper.ShowError("请选择位于仓库下的路径");
                else
                {
                    this.PointFolder.Text = folder.Path;
                    VM.EntryName = System.IO.Path.GetFileName(folder.Path);
                }
            }
        }

        //點擊封面
        private async void Button_CoverImg_Click(object sender, RoutedEventArgs e)
        {
            var file = await Helpers.PickHelper.PickImgAsync();
            if (file != null)
            {
                VM.Entry.CoverImg = file.Path;
                Image_CoverImg.Source = new BitmapImage(new Uri(file.Path));
            }
        }



        private async void GetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(this.ddb.Content).Equals("无服务")) return;
            this.pr.IsActive = true;
            var entryInfo = new Dictionary<string, object>();
            this.btn_GetInfo.IsEnabled = false;
            entryInfo = await EntryInfoService.GetEntryInfoNet(this.VM.EntryName, Convert.ToString(this.ddb.Content));
            this.pr.IsActive = false;
            this.btn_GetInfo.IsEnabled = true;

            #region 基本信息
            entryInfo.TryGetValue("封面", out object stream_cover);
            if (stream_cover != null)
            {
                TempImageUtils.CreateTempImage((MemoryStream)stream_cover);
                VM.Entry.CoverImg = TempImageUtils.GetDefaultTempImage();
                Image_CoverImg.Source = ImgHelper.CreateBitmapImage((MemoryStream)stream_cover);
            }

            entryInfo.TryGetValue("评分", out object rate);
            this.VM.MyRating = Convert.ToDouble(rate);

            entryInfo.TryGetValue("上映日期", out object date);
            this.VM.ReleaseDate = Convert.ToDateTime(date ?? DateTime.Now);

            #endregion

            //entryInfo.TryGetValue("描述", out object description);
            //this.VM.EntryDetail.Metadata.Desc = Convert.ToString(description);
        }

        /// <summary>
        /// 编辑词条名的时候同步修改仓库路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VM.Entry.Name == null)
                VM.SetFullEntryPathByName((sender as TextBox).Text);
        }
    }
}