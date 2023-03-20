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
using NPOI.SS.Formula.Functions;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Extensions;
using OMDb.Core.Services;
using OMDb.Core.Services.PluginsService;
using OMDb.Core.Utils;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditEntryDialog : Page
    {
        public ViewModels.EditEntryViewModel VM { get; set; }
        public EditEntryDialog(Core.Models.Entry entry)
        {
            VM = new ViewModels.EditEntryViewModel(entry);
            this.InitializeComponent();

            var lst_label_lp = Core.Services.LabelPropertyService.GetAllLabel(DbSelectorService.dbCurrentId);
            var lst_label_lc = Core.Services.LabelService.GetAllLabel(DbSelectorService.dbCurrentId);
            var lpids = new List<string>();//属性标签
            var lcids = new List<string>();//分类标签
            if (entry != null) lpids = Core.Services.LabelPropertyService.GetLabelIdsOfEntry(entry.EntryId);
            if (entry != null) lcids = Core.Services.LabelService.GetLabelIdsOfEntry(entry.EntryId);

            //VM.Label_Property = new List<Models.LabelProperty>();
            if (lst_label_lp.Count > 0)
            {
                foreach (var item in lst_label_lp)
                {
                    var lp = new Models.LabelProperty(item);
                    //根据词条&属性数据の关联关系 设置 □√
                    if (lpids.Count > 0 && lpids.Contains(item.LPId))
                        lp.IsChecked = true;
                    VM.Label_Property.Add(lp);
                }
                var lstBaba = VM.Label_Property.Where(a => a.LPDb.Level == 1);
                int n = 2;//第1列 or 第2列
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                foreach (var item in lstBaba)
                {
                    //第一列
                    if (n % 2 == 0)
                    {
                        if (n != 2)
                        {
                            stack = new StackPanel();
                            stack.Orientation = Orientation.Horizontal;
                        }
                        //属性 -> 标题
                        TextBlock tBlock = new TextBlock()
                        {
                            Margin = new Thickness(8, 5, 0, 0),
                            Text = item.LPDb.Name + "：",
                            Width = 87
                        };
                        stack.Children.Add(tBlock);
                        //属性 -> 属性
                        var lbc = new LabelsProPertyControl();
                        lbc.Name = item.LPDb.Name;
                        lbc.StrSelectItem.Text = item.LPDb.Name;
                        lbc.LabelPropertyCollection = VM.Label_Property.Where(a => a.LPDb.ParentId.NotNullAndEmpty()).Where(a => item.LPDb.LPId == a.LPDb.ParentId);
                        stack.Children.Add(lbc);
                        n++;
                        if (lstBaba.Count() == n - 2)
                            stp.Children.Add(stack);
                    }
                    //第二列
                    else
                    {
                        //属性 -> 标题
                        TextBlock tBlock = new TextBlock()
                        {
                            Margin = new Thickness(-11, 5, 0, 0),
                            Text = item.LPDb.Name + "：",
                            Width = 87
                        };
                        stack.Children.Add(tBlock);
                        //属性 -> 属性
                        var lbc = new LabelsProPertyControl();
                        lbc.Name = item.LPDb.Name;
                        lbc.LabelPropertyCollection = VM.Label_Property.Where(a => a.LPDb.ParentId.NotNullAndEmpty()).Where(a => item.LPDb.LPId == a.LPDb.ParentId);
                        stack.Children.Add(lbc);
                        stp.Children.Add(stack);
                        n++;
                    }
                }
            }
            //封面不为空 -> 设置封面
            if (entry != null && entry.CoverImg != null) { Image_CoverImg.Source = new BitmapImage(new Uri(VM.Entry.CoverImg)); }

            //編輯 -> 設置存儲模式+存儲地址+設置詞條絕對地址
            if (entry != null)
            {
                switch (entry.SaveType)
                {
                    case '1':
                        {
                            this.SetFolder.IsChecked = true;
                            var result = Core.Services.EntrySourceSerivce.SelectEntrySource(entry.EntryId, entry.DbId, FileType.Folder);
                            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId).StoragePath;
                            if (s != null && !string.IsNullOrWhiteSpace(result?.FirstOrDefault()?.Path))
                                PointFolder.Text = s + (result?.FirstOrDefault()?.Path.ToString());
                            break;
                        }
                    case '2':
                        {
                            this.SetFile.IsChecked = true;
                            var result = Core.Services.EntrySourceSerivce.SelectEntrySource(entry.EntryId, entry.DbId, FileType.All);
                            break;
                        }
                    case '3':
                        {
                            this.Local.IsChecked = true;
                            break;
                        }
                    default:
                        this.SetFolder.IsChecked = true;
                        break;
                }
                VM.SetFullEntryPathByRelativePath(entry.Path);
                VM.ReleaseDate = (DateTimeOffset)entry.ReleaseDate;
            }



            //新增 -> 默認存儲模式為指定文件夾
            else
            {
                this.SetFolder.IsChecked = true;
            }

            if (PluginsBaseService.EntryInfos.Count() > 0)
            {
                this.ddb.Content = PluginsBaseService.EntryInfos.FirstOrDefault().GetType().Assembly.GetName().Name;
                var mf = new MenuFlyout();
                foreach (var item in PluginsBaseService.EntryInfos)
                {
                    MenuFlyoutItem mfl = new MenuFlyoutItem();
                    mfl.Text = item.GetType().Assembly.GetName().Name;
                    mfl.Click += (s, e) => { this.ddb.Content = mfl.Text; };
                    mf.Items.Add(mfl);
                }
                this.ddb.Flyout = mf;
            }
            else this.ddb.Content = "无服务";








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

            //保存按鈕
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                //新增 or 編輯
                if (entry == null)
                {
                    entry = content.VM.Entry;
                    //entry.Name = content.VM.EntryNames.FirstOrDefault(p => p.IsDefault)?.Name;
                    entry.Name = content.VM.EntryName;
                    entry.DbId = content.VM.SelectedEnrtyStorage?.StorageName;
                }
                else
                {
                    entry.CopyFrom(content.VM.Entry);
                    //entry.Name = content.VM.EntryNames.FirstOrDefault(p => p.IsDefault)?.Name;
                    entry.Name = content.VM.EntryName;
                }
                var entryDetail = new Models.EntryDetail()
                {
                    Entry = entry,
                    //Names = content.VM.EntryNames.ToObservableCollection(),
                    Labels = content.VM.Labels.Select(p => p.LabelDb).ToList(),
                    //FullCoverImgPath = content.VM.Entry.CoverImg,
                    FullEntryPath = content.VM.Entry.Path,
                };

                //根據存储模式，選擇存储地址
                if (content.SetFolder.IsChecked == true)
                {
                    entryDetail.Entry.SaveType = '1';
                    entryDetail.PathFolder = content.PointFolder.Text;
                }
                else if (content.SetFile.IsChecked == true)
                {
                    entryDetail.Entry.SaveType = '2';
                }
                else
                {
                    entryDetail.Entry.SaveType = '3';
                }

                //封面空
                if (content.VM.Entry.CoverImg == null || content.VM.Entry.CoverImg.Length <= 0)
                {
                    List<string> lstPath = new List<string>();
                    switch (entryDetail.Entry.SaveType)
                    {
                        //封面空 -> 尋找指定文件夾中圖片 -> 尋找指定文件夾中視頻縮略圖 -> 設置默認封面
                        case '1':
                            lstPath.Add(content.PointFolder.Text);
                            entryDetail.FullCoverImgPath = Services.CommonService.GetCoverByPath(lstPath, FileType.Folder);
                            break;
                        //封面空 -> 尋找指定地址中圖片 -> 尋找指定地址中視頻縮略圖 -> 設置默認封面
                        case '2':
                            //lstPath.AddRange()
                            entryDetail.FullCoverImgPath = Services.CommonService.GetCoverByPath(lstPath, FileType.Folder);
                            break;
                        //設置默認封面
                        case '3':
                            entryDetail.FullCoverImgPath = Services.CommonService.GetCoverByPath();
                            break;
                        default:
                            entryDetail.FullCoverImgPath = Services.CommonService.GetCoverByPath();
                            break;
                    }
                }
                else
                {
                    entryDetail.FullCoverImgPath = content.VM.Entry.CoverImg;
                }
                //保存：标签 -> 属性
                var lst = content.VM.Label_Property.Where(a => a.IsChecked == true).Select(a => a.LPDb).ToList();
                if (lst.Count > 0) entryDetail.Lpdbs.AddRange(lst);




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
                if (!folder.Path.StartsWith(VM.SelectedEnrtyStorage.StoragePath, StringComparison.OrdinalIgnoreCase))
                    Helpers.DialogHelper.ShowError("请选择位于仓库下的路径");
                else
                {
                    this.PointFolder.Text = folder.Path;
                    VM.EntryName = folder.Path.SubString_A21("\\", 1, false, false);
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



        private void GetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(this.ddb.Content).Equals("无服务")) return;
            /*var rate=RatingService.GetRatings(this.VM.EntryName,Convert.ToString(this.ddb.Content));
            this.VM.MyRating = rate.Rate / rate.Max * 5.0;*/

            var entryInfo = EntryInfoService.GetEntryInfo(this.VM.EntryName, Convert.ToString(this.ddb.Content));
            try
            {
                var coverStream = (MemoryStream)(entryInfo["封面"]);
                TempFileHelper.CreateTempImg();
                FileStream fs = new FileStream(TempFileHelper.fullTempImgPath, FileMode.Create);
                coverStream.WriteTo(fs);
                fs.Close();
                VM.Entry.CoverImg = TempFileHelper.fullTempImgPath;
                Image_CoverImg.Source = ImgHelper.CreateBitmapImage(coverStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                this.VM.MyRating = Convert.ToDouble(entryInfo["评分"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                this.VM.ReleaseDate = Convert.ToDateTime(entryInfo["上映日期"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                var lstBaba = VM.Label_Property.Where(a => a.LPDb.Level == 1).ToList();
                foreach (var ei in entryInfo)
                {

                    if (lstBaba.Select(a=>a.LPDb.Name).ToList().Contains(ei.Key))
                    {
                        string[] eiv=(string[])ei.Value;
                        var lbc = (LabelsProPertyControl)this.stp.FindChild(ei.Key);
                        lbc.StrSelectItem.Text = string.Join("/", eiv);
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

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