using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Extensions;
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
using static OMDb.WinUI3.MyControls.LabelsControl;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditEntryDialog : Page
    {
        public ViewModels.EditEntryViewModel VM { get; set; }
        public EditEntryDialog(Core.Models.Entry entry)
        {
            VM = new ViewModels.EditEntryViewModel(entry);
            this.InitializeComponent();

            //取屬性標簽
            var lst_label_property = Core.Services.LabelService.GetAllLabel(DbSelectorService.dbCurrentId, true);
            var labelIds = new List<string>();
            if (entry != null) labelIds = Core.Services.LabelService.GetLabelIdsOfEntry(entry.EntryId);
            VM.Label_Property = new List<Models.Label>();
            if (lst_label_property.Count > 0)
            {
                foreach (var item in lst_label_property)
                {
                    var label = new Models.Label(item);
                    if (labelIds.Count > 0 && labelIds.Contains(item.Id)) label.IsChecked = true;
                    VM.Label_Property.Add(label);
                }
                var lstBaba = VM.Label_Property.Where(a => !a.LabelDb.ParentId.NotNullAndEmpty()).DepthClone<List<Models.Label>>();
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
                            Text = item.LabelDb.Name + "：",
                            Width = 87
                        };
                        stack.Children.Add(tBlock);
                        //属性 -> 属性
                        var lbc = new LabelsControl2();
                        lbc.Labels = VM.Label_Property.Where(a => a.LabelDb.ParentId.NotNullAndEmpty()).Where(a => item.LabelDb.Id == a.LabelDb.ParentId);
                        stack.Children.Add(lbc);
                        n++;
                        if (lstBaba.Count == n - 2)
                            stp.Children.Add(stack);
                    }
                    //第二列
                    else
                    {
                        //属性 -> 标题
                        TextBlock tBlock = new TextBlock()
                        {
                            Margin = new Thickness(-11, 5, 0, 0),
                            Text = item.LabelDb.Name + "：",
                            Width = 87
                        };
                        stack.Children.Add(tBlock);
                        //属性 -> 属性
                        var lbc = new LabelsControl2();
                        lbc.Labels = VM.Label_Property.Where(a => a.LabelDb.ParentId.NotNullAndEmpty()).Where(a => item.LabelDb.Id == a.LabelDb.ParentId);
                        stack.Children.Add(lbc);
                        stp.Children.Add(stack);
                        n++;
                    }
                }
            }
            //封面不为空 -> 设置封面
            if (entry != null && entry.CoverImg != null) { Image_CoverImg.Source = new BitmapImage(new Uri(VM.Entry.CoverImg)); }

            //編輯 -> 設置存儲模式+存儲地址
            if (entry != null)
            {
                switch (entry.SaveType)
                {
                    case '1':
                        {
                            this.SetFolder.IsChecked = true;
                            var result = Core.Services.EntrySourceSerivce.SelectEntrySource(entry.EntryId, entry.DbId, FileType.Folder);
                            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId).StoragePath;
                            if (s != null && !string.IsNullOrEmpty(entry.Path))
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
            }

            //新增 -> 默認存儲模式為指定文件夾
            else
            {
                this.SetFolder.IsChecked = true;
            }
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

                //根據存储模式，選擇存储地址和默認封面
                if (content.SetFolder.IsChecked == true)
                {
                    entryDetail.Entry.SaveType = '1';
                    entryDetail.PathFolder = content.PointFolder.Text;
                    //封面空 -> 尋找指定文件夾中圖片 -> 尋找指定文件夾中視頻縮略圖 -> 設置默認封面
                    if (content.VM.Entry.CoverImg == null || content.VM.Entry.CoverImg.Length <= 0)
                    {
                        entryDetail.LoadPathFolder();
                        if (entryDetail.Imgs.Count > 0)
                        {
                            entryDetail.FullCoverImgPath = entryDetail.Imgs[0];
                        }
                        else if (entryDetail.VideoExplorerItems.Count > 0)
                        {
                            entryDetail.FullCoverImgPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "defaultStorageCover.jpg");
                        }
                        else
                        {
                            entryDetail.FullCoverImgPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "defaultStorageCover.jpg");
                        }
                        entryDetail.Entry.CoverImg = entryDetail.FullCoverImgPath;
                    }
                }
                else if (content.SetFile.IsChecked == true)
                {
                    entryDetail.Entry.SaveType = '2';
                    //封面空 -> 尋找指定地址中圖片 -> 尋找指定地址中視頻縮略圖 -> 設置默認封面
                    if (content.VM.Entry.CoverImg == null || content.VM.Entry.CoverImg.Length <= 0)
                    {
                        entryDetail.FullCoverImgPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "defaultStorageCover.jpg");
                        entryDetail.Entry.CoverImg = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "defaultStorageCover.jpg");
                    }
                }
                else
                {
                    entryDetail.Entry.SaveType = '3';
                    //封面空 -> 設置默認封面
                    if (content.VM.Entry.CoverImg == null || content.VM.Entry.CoverImg.Length <= 0)
                    {
                        entryDetail.FullCoverImgPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "defaultStorageCover.jpg");
                        entryDetail.Entry.CoverImg = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "defaultStorageCover.jpg");
                    }
                }

                //保存：标签 -> 属性
                var lst = content.VM.Label_Property.Where(a => a.IsChecked == true).Select(a => a.LabelDb).ToList();
                entryDetail.Labels.AddRange(lst);




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
        /// <summary>
        /// 编辑词条名的时候同步修改仓库路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VM.SetEntryPath((sender as TextBox).Text);
        }
    }
}