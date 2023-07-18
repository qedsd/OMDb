using ColorCode.Compilation.Languages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NLog;
using NPOI.SS.Formula.Functions;
using OMDb.Core.Services.PluginsService;
using OMDb.Core.Utils;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.Services.Settings;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Net.WebRequestMethods;
using OMDb.Core.Services;
using OMDb.Core.Utils.Extensions;
using OMDb.Core.Utils.PathUtils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEntryBatchDialog : Page
    {
        public AddEntryBatchViewModel VM = new AddEntryBatchViewModel();

        public ViewModels.EditEntryViewModel VM_Detail = new EditEntryViewModel(null);
        public AddEntryBatchDialog()
        {
            this.InitializeComponent();
            this.LoadLabelControl();
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
                    mfl.Width = 200;
                }
                this.ddb.Flyout = mf;
            }
            else this.ddb.Content = "无服务";

        }

        public static async Task<List<EntryDetail>> ShowDialog()
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "词条批量新增";
            dialog.PrimaryButton.Content = "确认";
            dialog.CancelButton.Content = "取消";
            AddEntryBatchDialog content = new AddEntryBatchDialog();
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        private void SelectFolders_Click(object sender, RoutedEventArgs e)
        {
            var path = VM.SelectedEnrtyStorage.StoragePath;
            //this.ei.BasicGridView.ItemsSource= FileHelper.FindFolderItems(path).FirstOrDefault().Children.ToObservableCollection();
            this.ei.VM.Root = FileHelper.FindFolderItems(path).FirstOrDefault();
            this.ei.VM.CurrentFileInfos = new ObservableCollectionEx<ExplorerItem>(this.ei.VM.Root.Children);
            this.ei.VM.PathStack.Clear();
            this.ei.VM.PathStack.Add(System.IO.Path.GetFileName(path));
            this.flyGrid.Visibility = Visibility.Visible;
        }

        private void ReturnFolder_Click(object sender, RoutedEventArgs e)
        {
            var result = this.ei.BasicGridView.SelectedItems.ToList();
            if (!result.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                foreach (var item in result)
                {
                    ExplorerItem exp = item as ExplorerItem;
                    EntryDetail ed = new EntryDetail();
                    ed.FullCoverImgPath = Services.CommonService.GetCover(exp.Name);
                    ed.Name = System.IO.Path.GetFileName(exp.Name);
                    ed.FullEntryPath = PathService.GetFullEntryPathByEntryName(exp.Name, VM.SelectedEnrtyStorage.StoragePath);
                    ed.Entry = new Core.Models.Entry();
                    ed.Entry.DbId = VM.SelectedEnrtyStorage.StorageName;
                    ed.PathFolder = ((OMDb.WinUI3.Models.ExplorerItem)item).FullName;
                    VM.EntryDetailCollection.Add(ed);
                }
            }
            this.btn_FolderPicker.Flyout.Hide();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ei.BasicGridView.SelectedItems.Clear();
            this.btn_FolderPicker.Flyout.Hide();
        }



        private async void GetInfoBatch_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(this.ddb.Content).Equals("无服务")) return;
            //this.pr.IsActive = true;
            //this.btn_GetInfoBatch.IsEnabled = false;
            //(ObservableCollection<Models.EntryDetail>)
            Stopwatch swTotal = new Stopwatch();
            swTotal.Start();
            foreach (EntryDetail item in this.ldv_edc.SelectedItems)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var entryInfo = new Dictionary<string, object>();

                entryInfo = await EntryInfoService.GetEntryInfo(item.Name, Convert.ToString(this.ddb.Content));

                try
                {
                    var coverStream = (MemoryStream)(entryInfo["封面"]);
                    var newFileName = item.Name + ".jpg";
                    TempPathUtils.CreateTempImage(newFileName, coverStream);
                    item.FullCoverImgPath = TempPathUtils.GetTempFile(newFileName);
                }
                catch (Exception ex)
                {
                    Core.Utils.Logger.Error("封面" + ex.Message);
                }

                try
                {
                    item.Entry.MyRating = Convert.ToDouble(entryInfo["评分"]);
                    //item.Rate = Convert.ToDouble(entryInfo["评分"]);
                }
                catch (Exception ex)
                {
                    Core.Utils.Logger.Error("评分" + ex.Message);
                }

                try
                {
                    item.Entry.ReleaseDate= Convert.ToDateTime(entryInfo["上映日期"]);
                    //item.Date = Convert.ToDateTime(entryInfo["上映日期"]).ToShortDateString();
                }
                catch (Exception ex)
                {
                    Core.Utils.Logger.Error("上映日期" + ex.Message);
                }
                sw.Stop();
                Core.Utils.Logger.Info($"获取“{item.Name}”信息耗时：{sw.Elapsed}");
            }
            swTotal.Stop();
            Core.Utils.Logger.Info($"获取信息总耗时：{swTotal.Elapsed}");
            //this.pr.IsActive = false;
            //this.btn_GetInfo.IsEnabled = true;
        }

        private void ldv_edc_click(object sender, ItemClickEventArgs e)
        {

        }

        private void EntryName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_CoverImg_Click(object sender, RoutedEventArgs e)
        {

        }


        private void LoadLabelControl()
        {
            var lst_label_lp = Core.Services.LabelPropertyService.GetAllLabel(DbSelectorService.dbCurrentId);
            var lst_label_lc = Core.Services.LabelClassService.GetAllLabel(DbSelectorService.dbCurrentId);
            var lpids = new List<string>();//属性标签
            var lcids = new List<string>();//分类标签

            if (lst_label_lp.Count > 0)
            {
                foreach (var item in lst_label_lp)
                {
                    var lp = new Models.LabelProperty(item);
                    //根据词条&属性数据の关联关系 设置 □√
                    if (lpids.Count > 0 && lpids.Contains(item.LPId))
                        lp.IsChecked = true;
                    VM_Detail.Label_Property.Add(lp);
                }
                var lstBaba = VM_Detail.Label_Property.Where(a => a.LPDb.Level == 1);
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
                        lbc.LabelPropertyCollection = VM_Detail.Label_Property.Where(a => a.LPDb.ParentId.NotNullAndEmpty()).Where(a => item.LPDb.LPId == a.LPDb.ParentId).ToObservableCollection<LabelProperty>(); ;
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
                        lbc.LabelPropertyCollection = VM_Detail.Label_Property.Where(a => a.LPDb.ParentId.NotNullAndEmpty()).Where(a => item.LPDb.LPId == a.LPDb.ParentId).ToObservableCollection<LabelProperty>();
                        stack.Children.Add(lbc);
                        stp.Children.Add(stack);
                        n++;
                    }
                }
            }

        }

        private void ldv_edc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ldv_edc.SelectedItems.Count > 1) return;
            if (this.ldv_edc.SelectedItems.Count < 1)
            {
                this.grid_entryDetail.Visibility = Visibility.Collapsed;
                return;
            }
            this.grid_entryDetail.Visibility = Visibility.Visible;
            var ed = this.ldv_edc.SelectedItems.FirstOrDefault() as EntryDetail;
            //this.grid_entryDetail.Visibility = Visibility.Visible;
            this.VM_Detail.Entry = ed.Entry;
            this.VM_Detail.EntryDetail = ed;

            this.tb_ctmc.Text = this.VM_Detail.EntryDetail.Name;//词条名称
            this.tb_ctlj.Text = this.VM_Detail.EntryDetail.FullEntryPath; ;//词条路径
            this.tb_ccms.Text = "指定文件夹";//存储模式
            this.tb_zylj.Text = this.VM_Detail.EntryDetail.PathFolder;//资源路径
            this.Image_CoverImg.Source = this.VM_Detail.EntryDetail.FullCoverImgPath;
            //this.ccms.Text = "folder"+ DateTime.Now;
        }


    }
}
